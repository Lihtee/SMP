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
    
    public partial class Project
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Project()
        {
            this.isDone = false;
        }
    
        public int IdProject { get; set; }
        public string projectName { get; set; }
        public System.DateTime startDateTime { get; set; }
        public System.DateTime endDateTime { get; set; }
        public string description { get; set; }
        public bool isDone { get; set; }
    
        public virtual Project parrentProject { get; set; }
    }
}
