using System;
using Power.Business.Sessions;
using Power.Global;
using Power.IBaseCore;

namespace Power.JJ.APP
{
    // Token: 0x0200000B RID: 11
    public class PublicUtil : ISessionUtil
    {
        ISession ISessionUtil.getSession(bool nullAndCreate)
        {
            throw new NotImplementedException();
        }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000048 RID: 72 RVA: 0x000021A7 File Offset: 0x000003A7
		// (set) Token: 0x06000049 RID: 73 RVA: 0x000021A7 File Offset: 0x000003A7
		public ISession Session
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x0600004A RID: 74 RVA: 0x0000669C File Offset: 0x0000489C
		public ISession getSession(bool nullAndCreate = false)
		{
			string text = "ss_" + PublicUtil.SessionID;
			bool flag = text == null;
			bool flag2 = flag;
			ISession result;
			if (flag2)
			{
				result = null;
			}
			else
			{
				ISession session = null;
				bool flag3 = PowerGlobal.Cache.Exists(text);
				bool flag4 = flag3;
				if (flag4)
				{
					session = PowerGlobal.Cache.Get<ISession>(text);
				}
				bool flag5 = session == null && nullAndCreate;
				bool flag6 = flag5;
				if (flag6)
				{
					session = new Session();
					session.SessionId = text;
				}
				result = session;
			}
			return result;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000021AE File Offset: 0x000003AE
		public void setSession(ISession session)
		{
			throw new NotImplementedException();
		}

		// Token: 0x04000014 RID: 20
		public static string SessionID;
	}
}
