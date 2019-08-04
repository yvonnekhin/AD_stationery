using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace stationeryapp.Models
{
    public class ConsolidatedRequest
    {
        public string binNumber;
        public string description;
        public int totalNeeded;
        public int totalRetrieved;
        public List<DepartmentRequest> requests;

        public ConsolidatedRequest(string binNumber, string description)
        {
            this.binNumber = binNumber;
            this.description = description;
            this.totalNeeded = 0;
            this.totalRetrieved = 0;
            this.requests = new List<DepartmentRequest>();
        }

        public void addNeeded(int n)
        {
            totalNeeded += n;
        }

        public void addRetrieved(int r)
        {
            totalRetrieved += r;
        }

        public class DepartmentRequest
        {
            public string departmentCode;
            public int needed;
            public int actual;

            public DepartmentRequest(string departmentCode, int needed, int actual)
            {
                this.departmentCode = departmentCode;
                this.needed = needed;
                this.actual = actual;
            }
        }
    }
}