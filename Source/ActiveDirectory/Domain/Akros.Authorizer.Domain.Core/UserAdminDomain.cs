using Akros.Authorizer.Domain.Entities;
using Akros.Authorizer.Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Akros.Authorizer.Domain.Core
{
    public class UserAdminDomain
    {
        private readonly IActiveDirectoryRepository _activeDirectoryRepository;
        public UserAdminDomain(IActiveDirectoryRepository activeDirectoryRepository)
        {
            _activeDirectoryRepository = activeDirectoryRepository;
        }

        public async Task<bool> CreateUserAsync(CreateAdUserModel createAdUserModel)
        {
            createAdUserModel.UserManagerAccess = new AccessProperties
            {
                LDap = GetLdapConections(createAdUserModel.Country).Ldap,
                UserName = "felipe.alpizar@multimoney.com",
                Password = "FabianaSamuel22."
            };

            return await _activeDirectoryRepository.CreateUserAsync(createAdUserModel);
        }

        public LdapConection GetLdapConections(string country)
        {
            List<LdapConection> ldapConections = new List<LdapConection>
            {
                new LdapConection{Active = true, Country = "MX", Ldap ="LDAP://mx.f2p.in/DC=mx, DC=f2p, DC=in"},
                new LdapConection{Active = false, Country = "CR", Ldap="LDAP://172.29.1.91/DC=cr, DC=f2p, DC=in"},
                new LdapConection{Active = true, Country = "AKROS", Ldap ="LDAP://172.29.1.10/DC=Akros, DC=local"},
                new LdapConection{Active = false, Country ="NI", Ldap="LDAP://172.29.1.94/DC=ni, DC=f2p, DC=in"},
                new LdapConection{Active = true, Country ="GT", Ldap ="LDAP://172.29.1.92/DC=gt, DC=f2p, DC=in"},
                new LdapConection{Active = false,Country="SV", Ldap="LDAP://172.29.1.93/DC=sv, DC=f2p, DC=IN"}
            };

            var ldap = ldapConections.Find(x => x.Country == country);
            if (ldap != null)
            {
                return ldap;
            }

            throw new Exception($"LDAP No configurado para el pais {country}");
        }
    }
}
