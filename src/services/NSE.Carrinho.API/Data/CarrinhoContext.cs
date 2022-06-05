using Microsoft.EntityFrameworkCore;
using NSE.Carrinho.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NSE.Carrinho.API.Data
{
    public class CarrinhoContext : DbContext
    {
        public CarrinhoContext(DbContextOptions<CarrinhoContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbSet<CarrinhoItem> CarrinhoItems { get; set; }

        public DbSet<CarrinhoCliente> CarrinhoClientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("nse");

            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            modelBuilder.Entity<CarrinhoCliente>()
                .HasIndex(c => c.ClienteId)
                .HasName("IDX_Cliente");

            modelBuilder.Entity<CarrinhoCliente>()
                .HasMany(c => c.Itens)
                .WithOne(i => i.CarrinhoCliente)
                .HasForeignKey(c => c.CarrinhoId);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;
        }
    }
}