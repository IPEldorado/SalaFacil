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
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace ExchangeBackEndApp.Controllers
{
    public class LDAPController : Controller
    {
        //
        // POST: /LDAP/UserSession
        [HttpPost]
        public void UserSession()
        {
            String login = Request.Form["username"];
            String password = Request.Form["password"];

            if (AutenticaCredenciaisRede(login, password))
            {
                Response.Write("<span>Login successful.</span>");
            } else {            
                Response.Write("<span>Login failed.</span>");
            }
            Response.Write("<p />");

            // Allow access EWS from localhost 
            Response.AppendHeader("Access-Control-Allow-Origin", "*");
            Response.End();
        }

        /*
         * AutenticaCredenciaisRede
         * autenticates login/password over an LDAP server
         */
        private static bool AutenticaCredenciaisRede(string login, string senha)
        {
            DirectorySearcher searcher;
            bool credenciaisValidas;

            var entry = new DirectoryEntry(
                "LDAP://serv051/OU=users_funcionarios,OU=eldorado_user,DC=eldorado,DC=org,DC=br",
                login + "@" + "eldorado.org.br",
                senha);

            try
            {
                searcher = new DirectorySearcher(entry) { Filter = "(SAMAccountName=" + login + ")" };
                searcher.PropertiesToLoad.Add("cn");
                var result = searcher.FindOne();

                credenciaisValidas = (result != null);
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                entry.Close();
            }

            return credenciaisValidas;
        }
    }
}
