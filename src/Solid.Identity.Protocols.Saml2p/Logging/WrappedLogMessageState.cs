using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Logging
{
    internal class WrappedLogMessageState : LogMessageState
    {
        private object _state;

        public WrappedLogMessageState(object state) => _state = state;

        public override string ToString()
            => Serialize(_state, _state.GetType());
    }
}
