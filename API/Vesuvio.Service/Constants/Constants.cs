using System;
using System.Collections.Generic;
using System.Text;

namespace Vesuvio.Service
{

    public static class Constants
    {
        public static class ErrorCodes
        {
            public const string BadRequest = "Bad request", UnAuthorized = "Unauthorized", InvalidCredentials = "Invalid Credentials";
        }

        public static class Operation
        {
            public const string Insert = "I", Update = "U", Assigned = "Assigned", Approve = "Approved", Reject = "Rejected", New = "New", InReview = "InReview", WaitingForApproval = "WaitingForApproval", ReviewedByAdmin = "ReviewedByAdmin", InProgress = "InProgress";
            public const string Admin = "admin", SuperAdmin = "superadmin", Customer = "customer";
        }


    }
}
