﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Gate.Utils;

namespace Gate.AspNet
{
    public class AppHandler
    {
        readonly AppDelegate _app;

        public AppHandler(AppDelegate app)
        {
            _app = app;
        }

        public IAsyncResult BeginProcessRequest(HttpContextBase httpContext, AsyncCallback callback, object state)
        {
            var taskCompletionSource = new TaskCompletionSource<Action>(state);
            if (callback != null)
                taskCompletionSource.Task.ContinueWith(task => callback(task), TaskContinuationOptions.ExecuteSynchronously);

            var httpRequest = httpContext.Request;
            var serverVariables = new ServerVariables(httpRequest.ServerVariables);

            var pathBase = httpRequest.ApplicationPath;
            if (pathBase == "/" || pathBase == null)
                pathBase = "";

            var path = httpRequest.Path;
            if (path.StartsWith(pathBase))
                path = path.Substring(pathBase.Length);

            var env = new Dictionary<string, object>();

            var requestHeaders = httpRequest.Headers.AllKeys
                .ToDictionary(x => x, x => httpRequest.Headers.Get(x), StringComparer.OrdinalIgnoreCase);

            new Environment(env)
            {
                Version = "1.0",
                Method = httpRequest.HttpMethod,
                Scheme = httpRequest.Url.Scheme,
                PathBase = pathBase,
                Path = path,
                QueryString = serverVariables.QueryString,
                Headers = requestHeaders,
                Body = Body.FromStream(httpRequest.InputStream),
            };
            env["aspnet.HttpContextBase"] = httpContext;
            foreach (var kv in serverVariables.AddToEnvironment())
            {
                env["server." + kv.Key] = kv.Value;
            }

            _app.Invoke(
                env,
                (status, headers, body) =>
                {
                    try
                    {
                        httpContext.Response.BufferOutput = false;

                        httpContext.Response.Status = status;
                        foreach (var header in headers.SelectMany(kv => kv.Value.Split("\r\n".ToArray(), StringSplitOptions.RemoveEmptyEntries).Select(v => new {kv.Key, Value = v})))
                        {
                            httpContext.Response.AddHeader(header.Key, header.Value);
                        }
                        if (body == null)
                        {
                            taskCompletionSource.SetResult(() => httpContext.Response.End());
                            return;
                        }

                        var stream = httpContext.Response.OutputStream;
                        body(
                            (data, continuation) =>
                            {
                                if (continuation == null)
                                {
                                    stream.Write(data.Array, data.Offset, data.Count);
                                    return false;
                                }
                                var sr = stream.BeginWrite(data.Array, data.Offset, data.Count, ar =>
                                {
                                    if (ar.CompletedSynchronously) return;
                                    try
                                    {
                                        stream.EndWrite(ar);
                                    }
                                    catch (Exception ex)
                                    {
                                        taskCompletionSource.SetException(ex);
                                    }
                                    continuation();
                                }, null);
                                if (sr.CompletedSynchronously)
                                {
                                    stream.EndWrite(sr);
                                    return false;
                                }

                                return true;                                
                            },
                            ex=>taskCompletionSource.SetException(ex),
                            () => taskCompletionSource.SetResult(() => httpContext.Response.End()));
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.SetException(ex);
                    }
                },
                ex=>taskCompletionSource.SetException(ex));
            return taskCompletionSource.Task;
        }

        public void EndProcessRequest(IAsyncResult asyncResult)
        {
            var task = ((Task<Action>) asyncResult);
            task.Result.Invoke();
        }

        class ServerVariables
        {
            readonly NameValueCollection _serverVariables;

            public ServerVariables(NameValueCollection serverVariables)
            {
                _serverVariables = serverVariables;
            }

            public string QueryString
            {
                get { return _serverVariables.Get("QUERY_STRING"); }
            }

            public string ServerName
            {
                get { return _serverVariables.Get("SERVER_NAME"); }
            }

            public string ServerPort
            {
                get { return _serverVariables.Get("SERVER_PORT"); }
            }

            public IEnumerable<KeyValuePair<string, object>> AddToEnvironment()
            {
                return _serverVariables
                    .AllKeys
                    .Where(key => !key.StartsWith("HTTP_"))
                    .Select(key => new KeyValuePair<string, object>(key, _serverVariables.Get(key)));
            }
        }
    }
}