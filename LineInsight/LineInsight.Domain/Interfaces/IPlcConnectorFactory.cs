using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LineInsight.Domain.Entities;

namespace LineInsight.Domain.Interfaces;

public interface IPlcConnectorFactory
{
    IPlcConnector Create(PlcEndpoint endpoint);
}
