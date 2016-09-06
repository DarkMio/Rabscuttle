using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkPreviewPlugin {
    interface ILinkHandler {

        string GenerateResponse(string url);
        bool ReactsToUrl(string url);
    }
}
