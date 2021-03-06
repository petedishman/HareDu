﻿// Copyright 2013-2014 Albert L. Hives, Chris Patterson, et al.
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

namespace HareDu.Internal
{
    using System;
    using Contracts;
    using Newtonsoft.Json;

    internal class UserCharacteristicsImpl :
        UserCharacteristics
    {
        public UserCharacteristicsImpl()
        {
            Tags = UserPermissionTag.None;
        }

        [JsonProperty(PropertyName = "password", Order = 1, Required = Required.Always)]
        public string Password { get; set; }

        [JsonProperty(PropertyName = "tags", Order = 2, Required = Required.Default)]
        public string Tags { get; set; }

        public void WithPassword(string password)
        {
            Password = password;
        }

        public void WithTags(Action<UserAccessCharacteristics> tags)
        {
            var action = new UserAccessCharacteristicsImpl();
            tags(action);
            Tags = action.ToString();
        }
    }
}