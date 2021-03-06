﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Owin;
using Kayak;

namespace Gate.Hosts.Kayak
{
    class DataProducer : IDataProducer
    {
        readonly Func<Stream, Task> del;

        public DataProducer(Func<Stream, Task> del)
        {
            this.del = del;
        }

        public IDisposable Connect(IDataConsumer channel)
        {
            throw new NotImplementedException();
            /*
            var cts = new CancellationTokenSource();
            del(
                (data, continuation)=>
                {
                    if (channel.OnData(data, () => continuation(null)) == true)
                    {
                        return OwinConstants.CompletingAsynchronously;
                    }
                    return OwinConstants.CompletedSynchronously;
                },
                error =>
                {
                    if (error == null) 
                        channel.OnEnd(); 
                    else 
                        channel.OnError(error);
                },
                cts.Token);
            return new Disposable(cts.Cancel);*/
        }
    }
}
