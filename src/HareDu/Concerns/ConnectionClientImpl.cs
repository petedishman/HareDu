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
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Async;
    using Model;

    internal class ConnectionClientImpl :
        HareDuClientBase,
        ConnectionClient
    {
        public ConnectionClientImpl(ClientCharacteristicsImpl args) : base(args)
        {
            Channel = new ChannelClientImpl(args);
        }

        public ChannelClient Channel { get; private set; }

        public Task<IEnumerable<Connection>> GetAll(CancellationToken cancellationToken = default(CancellationToken))
        {
            LogInfo("Sent request to return all information pertaining to all connections on current RabbitMQ server.");

            string url = "api/connections";

            return base.Get(url, cancellationToken).As<IEnumerable<Connection>>(cancellationToken);
        }

        public Task<Connection> Get(string connectionName,
                                    CancellationToken cancellationToken = default(CancellationToken))
        {
            connectionName.Validate("connectionName", () => LogError(GetArgumentNullExceptionMsg, "Connection.Get"));

            string url = string.Format("api/connections/{0}", connectionName);

            LogInfo(
                string.Format(
                    "Sent request to return all information pertaining to connection '{0}' on current RabbitMQ server.",
                    connectionName));

            return base.Get(url, cancellationToken).As<Connection>(cancellationToken);
        }

        public Task<DeleteCmdResponse> Close(string connectionName,
                                             CancellationToken cancellationToken = default(CancellationToken))
        {
            connectionName.Validate("connectionName", () => LogError(GetArgumentNullExceptionMsg, "Connection.Close"));

            string url = string.Format("api/connections/{0}", connectionName);

            LogInfo(
                string.Format(
                    "Sent request to return all information pertaining to connection '{0}' on current RabbitMQ server.",
                    connectionName));

            return base.Delete(url, cancellationToken).Response<DeleteCmdResponse>(cancellationToken);
        }
    }
}