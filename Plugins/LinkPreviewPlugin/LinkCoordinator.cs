using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkPreviewPlugin;
using LinkPreviewPlugin.Handlers;

namespace LinkPreviewPlugin {
    class LinkCoordinator {

        private static readonly object SYNC_LOCK = new Object();
        private static LinkCoordinator _instance;
        private List<ILinkHandler> _handlers;

        private LinkCoordinator() {
            _handlers = new List<ILinkHandler>();
            _handlers.Add(new RedditHandler());
        }

        public static LinkCoordinator Instance {
            get {
                if (_instance == null) {
                    lock (SYNC_LOCK) {
                        if (_instance == null) {
                            _instance = new LinkCoordinator();
                        }
                    }
                }
                return _instance;
            }
        }


        public string GenerateResponse(string text) {
            foreach (ILinkHandler linkHandler in _handlers) {
                if (linkHandler.ReactsToUrl(text)) {
                    return linkHandler.GenerateResponse(text);
                }
            }
            return null;
        }
    }
}
