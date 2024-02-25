using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PayToPhone.Driver.App.Contracts {
    public interface IPayToPhoneListener {
        Task Startlistener();

        Task Stoplistener();

        Task<bool> GetlistenerStatus();
    }
}
