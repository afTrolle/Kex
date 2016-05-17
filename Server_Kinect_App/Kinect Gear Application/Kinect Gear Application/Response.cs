using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Gear_Application
{
    class Response
    {
        //if the function is successful return true
        //if the function failes or errors occur add error 
        public bool Status { get; set; }

        public string ErrorMessage { get; set; }

        public Object Data { get; set; }
        public Response(bool Status,  string ErrorMessage,  Object Data)
        {
            this.ErrorMessage = ErrorMessage;
            this.Status = Status;
            this.Data = Data;
        }


    }
}
