﻿// Copyright 2012-2013 Albert L. Hives, Chris Patterson, et al.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace HareDu
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Logging;

    public abstract class HareDuClientBase
    {
        protected Func<string, string> GetArgumentNullExceptionMsg =
            (msg) => string.Format("{0} method threw an ArgumentNullException exception because host URL was invalid (i.e. empty, null, or all whitespaces)", msg);
        protected Func<string, string> GetArgumentExceptionMsg =
            (msg) => string.Format("{0} method threw an ArgumentException exception because host URL was invalid (i.e. empty, null, or all whitespaces)", msg);

        protected HareDuClientBase(ClientCharacteristicsImpl args)
        {
            args.HostUrl.Validate("args.HostUrl", () => LogError(GetArgumentNullExceptionMsg, "HareDuClientBase"));
            args.Username.Validate("args.Username", () => LogError(GetArgumentNullExceptionMsg, "HareDuClientBase"));
            args.Password.Validate("args.Password", () => LogError(GetArgumentNullExceptionMsg, "HareDuClientBase"));
            args.VirtualHost.Validate("args.HostVirtualHostUrl", () => LogError(GetArgumentNullExceptionMsg, "HareDuClientBase"));

            Init = args;
            Logger = args.Logger;
            IsLoggingEnabled = !Logger.IsNull();
            Client = GetClient();
        }

        protected HttpClient Client { get; set; }
        protected ILog Logger { get; private set; }
        protected bool IsLoggingEnabled { get; private set; }
        protected ClientCharacteristicsImpl Init { get; private set; }

        /// <summary>
        /// Overrides default behaviour of System.Uri because RabbitMQ uses a forward slash, "/" , to represent the default virtual host.
        /// This method is just a workaround.
        /// </summary>
        private void LeaveDotsAndSlashesEscaped()
        {
            var getSyntaxMethod =
                typeof (UriParser).GetMethod("GetSyntax", BindingFlags.Static | BindingFlags.NonPublic);
            if (getSyntaxMethod == null)
            {
                throw new MissingMethodException("UriParser", "GetSyntax");
            }

            var uriParser = getSyntaxMethod.Invoke(null, new object[] {"http"});

            var setUpdatableFlagsMethod =
                uriParser.GetType().GetMethod("SetUpdatableFlags", BindingFlags.Instance | BindingFlags.NonPublic);
            if (setUpdatableFlagsMethod == null)
            {
                throw new MissingMethodException("UriParser", "SetUpdatableFlags");
            }

            setUpdatableFlagsMethod.Invoke(uriParser, new object[] {0});
        }

        protected HttpClient GetClient()
        {
            var client = new HttpClient(new HttpClientHandler
                                            {
                                                Credentials = new NetworkCredential(Init.Username, Init.Password)
                                            }) {BaseAddress = new Uri(string.Format("{0}/", Init.HostUrl))};
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (Init.Timeout != TimeSpan.Zero)
                client.Timeout = Init.Timeout;

            return client;
        }

        protected virtual Task<HttpResponseMessage> Get(string url, CancellationToken cancellationToken =
                                                                        default(CancellationToken))
        {
            try
            {
                if (url.Contains("/%2f"))
                    LeaveDotsAndSlashesEscaped();

                return Client.GetAsync(url, cancellationToken);
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }
        }

        protected virtual Task<HttpResponseMessage> Delete(string url, CancellationToken cancellationToken =
                                                                           default(CancellationToken))
        {
            try
            {
                if (url.Contains("/%2f"))
                    LeaveDotsAndSlashesEscaped();

                return Client.DeleteAsync(url, cancellationToken);
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }
        }

        protected virtual Task<HttpResponseMessage> Put<T>(string url, T value, CancellationToken cancellationToken =
                                                                                    default(CancellationToken))
        {
            try
            {
                if (url.Contains("/%2f"))
                    LeaveDotsAndSlashesEscaped();

                return Client.PutAsJsonAsync(url, value, cancellationToken);
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }
        }

        protected virtual Task<HttpResponseMessage> Post<T>(string url, T value, CancellationToken cancellationToken =
                                                                                     default(CancellationToken))
        {
            try
            {
                if (url.Contains("/%2f"))
                    LeaveDotsAndSlashesEscaped();

                return Client.PostAsJsonAsync(url, value, cancellationToken);
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }
        }

        protected virtual void LogError(Exception e)
        {
            if (IsLoggingEnabled)
                Logger.Error(x => x("[Msg]: {0}, [Stack Trace] {1}", e.Message, e.StackTrace));
        }

        protected virtual void LogError(string message)
        {
            if (IsLoggingEnabled)
                Logger.Error(message);
        }

        protected virtual void LogError(Func<string, string> getErrorMsg, string msgSource)
        {
            if (IsLoggingEnabled)
                Logger.Error(getErrorMsg(msgSource));
        }

        protected virtual void LogInfo(string message)
        {
            if (IsLoggingEnabled)
                Logger.Info(message);
        }
    }
}