using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SMP.Models;
using SMP.Models.Repositoryes;

namespace SMP.ViewModels
{
    /// <summary>
    /// Модель отчета о ходе проекта для личного кабинета менеджера.
    /// </summary>
    public class ManagerReportViewModel
    {
        public Project Project;

        public double CompletionRate;

        public int WorksCompleted;

        public int WorksInTotal;

        public double ReserveDaysRate;

        public int ReserveDaysUsed;

        public List<Project> TopLevelCompletedWorks;

        public ManagerReportViewModel(Project project)
        {
            var pr = new ProjectRepository(new ModelContainer());

            Project = project;
            var allTopChildren = pr.GetProjectsByParrentId(project.IdProject);
            var allLowestChildren = pr.GetLowestWorks(project);

            TopLevelCompletedWorks = allTopChildren.Where(proj => proj.isClose).ToList();

            WorksCompleted = allLowestChildren.Count(proj => proj.isClose);
            WorksInTotal = allLowestChildren.Count;
            CompletionRate = (double) WorksCompleted / WorksInTotal;

            var daysUsed = TopLevelCompletedWorks.Sum(work => (work.startDateTime - work.endDateTime).Days);
            ReserveDaysUsed = daysUsed - (project.endDateTime - project.startDateTime).Days - project.reserve;
            ReserveDaysRate = (double)ReserveDaysUsed / project.reserve;
        }
    }
}