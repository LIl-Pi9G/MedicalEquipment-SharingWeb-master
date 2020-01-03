using MedicalEquipment_Sharing.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicalEquipment_Sharing.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options) { }

		public DbSet<User> Users { get; set; }
		public DbSet<MedicalEquipment> MedicalEquipments { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<PayOrder> PayOrders { get; set; }

	}
}
