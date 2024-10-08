﻿using ClinicDentClientCommon.Interfaces;
using ClinicDentClientCommon.Model;
using System.Threading.Tasks;

namespace ClinicDentClientCommon.ViewModel
{
    public class ScheduleSummaryViewModel : BaseViewModel
    {
        public Doctor Doctor { get; set; }
        public int Price { get; set; }
        public int Payed { get; set; }
        public int Expenses { get; set; }
        public string DisplayText
        {
            get
            {
                return $"{Doctor.Name}: {Payed}/{Price} грн. Прибуток: {Price - Expenses} грн.";
            }
        }
        public async Task Initialize(Doctor doctor, int price, int payed, int expenses)
        {
            Doctor = doctor;
            Price = price;
            Payed = payed;
            Expenses = expenses;
        }
        public ScheduleSummaryViewModel(INavigate navigation) : base(navigation)
        {
        }
    }
}
