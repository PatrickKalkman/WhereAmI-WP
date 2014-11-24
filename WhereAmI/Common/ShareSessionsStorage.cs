using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhereAmI.Common
{
    public class ShareSessionsStorage
    {
        private const string SessionStorageFile = "WhereAmI.json";
        private Storage storage;

        public ShareSessionsStorage(Storage storage)
        {
            this.storage = storage;
        }

        public int GetNumberOfShareSessionsInTheLast24Hour()
        {
            List<DateTime> sessions = ReadSessions();
            return GetNumberOfSessionsInTheLast24Hour(sessions);
        }

        public int GetNumberOfShareSessions()
        {
            return ReadSessions().Count();
        }

        public void AddSession()
        {
            List<DateTime> sessions = ReadSessions();
            sessions.Add(DateTime.Now);
            StoreSessions(sessions);
        }

        private int GetNumberOfSessionsInTheLast24Hour(List<DateTime> sessions)
        {
            int numberOfSessions = 0;
            if (sessions != null)
            {
                return sessions.Count(s => s.Year == DateTime.Now.Year && s.DayOfYear == DateTime.Now.DayOfYear);
            }
            return numberOfSessions;
        }

        private List<DateTime> ReadSessions()
        {
            string sessionStorageFile = storage.Read(SessionStorageFile);
            if (!string.IsNullOrWhiteSpace(sessionStorageFile))
            {
                return JsonConvert.DeserializeObject<List<DateTime>>(sessionStorageFile);
            }
            else
            {
                return new List<DateTime>();
            }
        }

        private void StoreSessions(List<DateTime> sessionsToStore)
        {
            string sessions = JsonConvert.SerializeObject(sessionsToStore);
            storage.Write(SessionStorageFile, sessions);
        }

        public void Reset()
        {
            StoreSessions(new List<DateTime>());
        }
    }
}
