//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SMP.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Addiction
    {
        public int Id { get; set; }
    
        public virtual Project lastProject { get; set; }
        public virtual Project nextProject { get; set; }
    }
}