using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace JsonSample.NotPageController
{
   
    public class NotFoundViewResult : ViewResult
    {
        public NotFoundViewResult(string viewName)
        {
            ViewName = viewName;
            StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
    
}
