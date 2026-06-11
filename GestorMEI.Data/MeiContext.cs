using GestorMEI.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GestorMEI.Data
{
    public class MeiContext : DbContext
    {
        public MeiContext()
        {
            
        }
        public MeiContext(DbContextOptions<MeiContext> options) : base(options)
        {
        }

        public DbSet<AssinaturaModel> Assinatura { get; set; }
        public DbSet<EmpresaModel> Empresa { get; set; }
        public DbSet<TabelaGeralModel> TabelaGeral { get; set; }
        public DbSet<TabelaGeralItemModel> TabelaGeralItem { get; set; }
        public DbSet<VendaModel> Vendas { get; set; }
    }
}
