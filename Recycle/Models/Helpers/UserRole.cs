namespace Recycle.Models.Helpers
{
    /// <summary>
    /// Represents the role of a user (<see cref="User"/>).
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Marks a user role as customer.
        /// </summary>
        Customer = 0,

        /// <summary>
        /// Marks a user role as Seller.
        /// </summary>
        Seller = 1,

        ///<summary>
        ///Mark a user role as Admin
        ///</summary>
        Admin = 2,
        
    }
}
