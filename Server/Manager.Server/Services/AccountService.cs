using Manager.Core.Enums;
using Manager.Core.Models.Accounts;
using Manager.Core.Models.Users;
using Manager.Extensions;
using Manager.Infrastructure.IRepositoies;
using Manager.Server.IServices;
using System.Linq.Expressions;

namespace Manager.Server.Services
{
    public class AccountService : IAccountService
    {
        private readonly IBase baseService;

        private static readonly string privateKey = "-----BEGIN RSA PRIVATE KEY-----MIICXQIBAAKBgQDOW9ZsU93Ue4zzOVh8DWWnRO/+RfCOs1q8JvmKZqOvFdgY07SYqswGL9XpNlS3BUP/Ep7QanEMcqMgPimpplQWpgPDyq3M1hvmUUXCkfIAF1lyMNKx3w1vJ9NsCnIeNliOlv/WXkfOZeJ5R5LhIxYjNfKiczJ4AfbTiCqe1YWIMQIDAQABAoGBAJ8hfuamfcffRsBBFpUDF8K3jIJ+mJTShkPVolUx9UONCsmKaBfajd6vgLuIpCdGrjrCtyltC6RXuqegiCxFEU3JkcUUXvL2X8QpgnqNq7GrYgnzX87EzM2UXK58yp2dtijznIIjhcRy9zl6DIwepXci0hzG2dD6jYsDAAN1eFIJAkEA/gAiHWuYeCrqoXn5NFDUP6c1gAcxftqmTFlKUAd0y0t8+M4uJAdoL+9HGwQ4YtQf64mbydd6TfXE5hKsKSLawwJBAM/7shkxrJWwQOfBopAzxSEOZFYNiNeaugUId8X7C+4lrwtopkB481ob8MrVZy1cUGLUaqOwb8lOlOqtSVtUGfsCQEQyTPaROPKqsyx/z0UYnqQohNjHFab1lcjSAH3UQquCrR8wXHsX8gVMvU6np2wBgECBRe6/h/r+jcsoIEk7LnkCQQCZe/lWtl3SqZt8bF13ZX0Yg/JvvtU5pymYBUO+iyGmwZCILtZhxeBwoyXzycC2rOV1yaRY4B/ew2sKNI9qIop5AkB39hWm/Yj+tA+EobWOomrDEqOTB5g+pbYw/jqWHKClMo6W4lfRNExmEEyZfzF+XCKU1ufSLSyBzlgr2Schf2lm-----END RSA PRIVATE KEY-----";

        public AccountService(IBase baseService)
        {
            this.baseService = baseService;
        }

        public async Task<bool> CreateAccount(string name, string password, string phone, string nickName)
        {
            /*
             * 1.创建账号信息相关表
             * 2.事务更新
             */
            Dictionary<object, CrudEnum> dic = new();

            var UId = Guid.NewGuid();
            var dt = DateTime.Now;

            Account account = new()
            {
                Id = Guid.NewGuid(),
                UId = UId,
                Name = name,
                Phone = phone,
                Mail = "",
                Password = Md5Helper.MD5(password),
                Status = (sbyte)Status.Enable,
                Created = dt
            };
            dic.Add(account, CrudEnum.Create);

            AccountInfo accountInfo = new()
            {
                UId = UId,
                NickName = nickName,
                Sex = 0,
                Vip = 0,
                Location = "{}",
                Hometown = "{}",
                Company = "[]",
                School = "[]",
                Emotion = (sbyte)EmotionEnum.Unkown,
                Describe = "",
                Tag = "[]",
                OfficialCert = "[]",
                AvatarId = Guid.Empty,
                CoverId = Guid.Empty
            };
            dic.Add(accountInfo, CrudEnum.Create);

            UserGroup userGroup = new()
            {
                UId = UId,
                Grp = "[]"
            };
            dic.Add(userGroup, CrudEnum.Create);

            var res = await baseService.BatchTransactionAsync(dic);

            return res;
        }

        public async Task<Account> GetAccountBy(Expression<Func<Account, bool>> expression, bool isTrack = true)
        {
            var account = await baseService.FirstOrDefaultAsync(expression, isTrack);
            return account;
        }

        public async Task<Account> GetAccountBy(string phone, string password, bool isTrack = true)
        {
            var pwd = CryptoHelper.RSADecrypt(password, privateKey, "PEM");

            password = Md5Helper.MD5(pwd);

            var account = await baseService.FirstOrDefaultAsync<Account>((x => x.Password == password && x.Phone == phone), isTrack);

            return account;
        }

        public async Task<bool> ModifyAccount(Account account)
        {
            return await baseService.ModifyAsync(account) > 0;
        }

        public async Task<bool> ModifyAccountPassword(Expression<Func<Account, bool>> expression, string password)
        {
            var account = await baseService.FirstOrDefaultAsync(expression, true);
            if (account == null)
                return false;
            var DesPwd = CryptoHelper.RSADecrypt(password, privateKey, "PEM");
            //修改账号密码
            var psd = Md5Helper.MD5(DesPwd);
            account.Password = psd;
            return await baseService.ModifyAsync(account) > 0;
        }
    }
}