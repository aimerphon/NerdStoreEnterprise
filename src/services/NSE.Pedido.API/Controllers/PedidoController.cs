﻿using Microsoft.AspNetCore.Authorization;
using NSE.WebApi.Core.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSE.Pedidos.API.Controllers
{
    [Authorize]
    public class PedidoController : MainController
    {
    }
}