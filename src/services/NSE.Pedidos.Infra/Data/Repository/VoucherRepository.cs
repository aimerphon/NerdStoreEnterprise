﻿using Microsoft.EntityFrameworkCore;
using NSE.Core.Data;
using NSE.Pedidos.Domain.Vouchers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NSE.Pedidos.Infra.Data.Repository
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly PedidosContext _context;

        public VoucherRepository(PedidosContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<Voucher> ObterPorCodigo(string codigo)
        {
            return await _context.Vouchers.FirstOrDefaultAsync(voucher => voucher.Codigo == codigo);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}