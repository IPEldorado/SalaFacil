/*
Copyright (c) 2013, Colaboradores do INSTITUTO DE PESQUISAS ELDORADO
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided 
that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the 
following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the 
following disclaimer in the documentation and/or other materials provided with the distribution.

3. Neither the name of the INSTITUTO DE PESQUISAS ELDORADO nor the names of its contributors may be used to endorse or promote 
products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE 
USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using Microsoft.Exchange.WebServices.Data;

namespace Exchange101
{
    public interface IUserData
    {
        ExchangeVersion Version { get; }
        string EmailAddress { get; }
        SecureString Password { get; }
        Uri AutodiscoverUrl { get; set; }
    }

    public class UserDataFromConsole : IUserData
    {
        private static UserDataFromConsole userData;

        public static IUserData GetUserData()
        {
            if (userData == null)
            {
                GetUserDataFromConsole();
            }

            return userData;
        }

        private static void GetUserDataFromConsole()
        {
            userData = new UserDataFromConsole();

            userData.Password = new SecureString();
            userData.Password.MakeReadOnly();
        }

        public ExchangeVersion Version { get { return ExchangeVersion.Exchange2010_SP2; } }

        public string EmailAddress
        {
            get;
            set;
        }

        public SecureString Password
        {
            get;
            private set;
        }

        public Uri AutodiscoverUrl
        {
            get;
            set;
        }
    }
}
