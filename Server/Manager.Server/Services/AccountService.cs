﻿using Manager.Core.Enums;
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
                Status = (sbyte)Status.ENABLE,
                Created = dt
            };
            dic.Add(account, CrudEnum.CREATE);

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
                Emotion = (sbyte)EmotionEnum.UNKNOWN,
                Describe = "",
                Tag = "[]",
                OfficialCert = "[]",
                AvatarId = Guid.Empty,
                CoverId = Guid.Empty
            };
            dic.Add(accountInfo, CrudEnum.CREATE);

            UserGroup userGroup = new()
            {
                UId = UId,
                Grp = "[]"
            };
            dic.Add(userGroup, CrudEnum.CREATE);

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
            password = Md5Helper.MD5(password);

            var account = await baseService.FirstOrDefaultAsync<Account>((x => x.Password == password && x.Phone == phone), isTrack);

            return account;
        }

        public async Task<bool> ModifyAccount(Account account)
        {
            return await baseService.UpdateAsync(account) > 0;
        }

        public async Task<bool> ModifyAccountPassword(Expression<Func<Account, bool>> expression, string password)
        {
            var account = await baseService.FirstOrDefaultAsync(expression, true);
            if (account == null)
                return false;
            //修改账号密码
            var psd = Md5Helper.MD5(password);
            account.Password = psd;
            return await baseService.UpdateAsync(account) > 0;
        }
    }
}