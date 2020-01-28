using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHouse.DAL.Controller
{
    public interface IDocumentController
    {
        void InitDocument();
        void SaveDocument(string fileName);
        void DeleteTempFiles();
    }
}
