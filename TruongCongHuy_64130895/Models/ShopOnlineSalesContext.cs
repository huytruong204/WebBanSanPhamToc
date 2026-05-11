using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TruongCongHuy_64130895.Models;

public partial class ShopOnlineSalesContext : IdentityDbContext<Customers>
{
    public ShopOnlineSalesContext()
    {
    }

    public ShopOnlineSalesContext(DbContextOptions<ShopOnlineSalesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }
    public virtual DbSet<MENU> Menus { get; set; }

   
}
