using ERPService.DataModel.DTO;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPService.Data.AppAdmin
{
    public class UserMasterConfig : EntityTypeConfiguration<UserMaster>
    {
        public  UserMasterConfig()
        {
            this.ToTable("UserMaster");

            this.Property(s => s.Id).HasColumnName("Id")
               .IsRequired();

            this.Property(s => s.UserName).HasColumnName("UserName")
               .IsRequired()
               .HasMaxLength(20);
            this.Property(s => s.EmailId).HasColumnName("EmailId")
               .IsRequired()
               .HasMaxLength(50);
            this.Property(s => s.FirstName).HasColumnName("FirstName")
               .IsRequired()
               .HasMaxLength(50);
            this.Property(s => s.LastName).HasColumnName("LastName")
               .IsRequired()
               .HasMaxLength(50);
            this.Property(s => s.UserType).HasColumnName("UserType")
               .IsRequired()
               .HasMaxLength(1);
            this.Property(s => s.CreatedBy).HasColumnName("CreatedBy")
               .IsRequired();
            this.Property(s => s.CreatedDate).HasColumnName("CreatedOn")
               .IsRequired();
            this.Property(s => s.ModifiedBy).HasColumnName("ModifiedBy")
               .IsRequired();
            this.Property(s => s.ModifiedDate).HasColumnName("ModifiedOn")
               .IsRequired();
            this.Property(s => s.Active).HasColumnName("Active")
               .IsRequired()
               .HasMaxLength(1);
        }
    }
}
