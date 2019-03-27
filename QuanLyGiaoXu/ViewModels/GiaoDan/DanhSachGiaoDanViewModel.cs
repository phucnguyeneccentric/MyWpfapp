﻿using DataLayer;
using BusinessTier;
using MaterialDesignThemes.Wpf;
using QuanLyGiaoXu.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using EntityTier;
using System.Collections.ObjectModel;
using QuanLyGiaoXu.ViewModels.CacGioi.ChiTietGiaoDan;
using QuanLyGiaoXu.Views.GiaoDan;
using System.Windows.Data;
using QuanLyGiaoXu.Mediators;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;

namespace QuanLyGiaoXu.ViewModels.GiaoDan
{
    public class DanhSachGiaoDanViewModel : GiaoDanViewModelBase
    {
        #region fields
        public override string Name => GiaoXuHelper.DanhSachGiaoDan;
        public override string Icon => GiaoXuHelper.DanhSachGiaoDanIcon;
        private string _textSearch;
        private Nullable<int> _selectedGioi;
        private string _selectedGiaoHo;
        private ObservableCollection<string> _listGiaoHo;
        private ObservableCollection<string> _listGioi;
        private bool _removeGioifilter;
        private bool _removeGiaohofilter;
        private ICollectionView _ListGiaoDan;
        private ICommand _addGiaoDanCommand;

        #endregion

        #region Properties

        public ICollectionView ListGiaoDan
        {
            get { return _ListGiaoDan; }
            set
            {
                _ListGiaoDan = value;

                OnPropertyChanged("ListGiaoDan");
            }
        }

        public string TextSearch
        {
            get { return _textSearch; }
            set
            {
                _textSearch = value;

                ICollectionView view = this.ListGiaoDan;
                Predicate<object> p = (obj) =>
                   {
                       ChiTietGiaoDanViewModel giaodan = obj as ChiTietGiaoDanViewModel;
                       return giaodan.Ten.ToLower().Contains(_textSearch)
                           || giaodan.Ho.ToLower().Contains(_textSearch)
                           || giaodan.MaGiaoDan.Contains(_textSearch);

                   };
                view.Filter = p;
            }
        }
        public string SelectedGiaoHo
        {
            get { return _selectedGiaoHo; }
            set
            {
                if (_selectedGiaoHo == value)
                    return;
                _selectedGiaoHo = value;
                OnPropertyChanged("SelectedGiaoHo");
                //add code here
            }
        }
        public Nullable<int> SelectedGioi
        {
            get { return _selectedGioi; }
            set
            {
                if (_selectedGioi == value)
                    return;
                _selectedGioi = value;
                OnPropertyChanged("SelectedGioi");
                //add code
            }
        }
        
        public ObservableCollection<string> ListGiaoHo
        {
            get { return _listGiaoHo; }
            set
            {
                if (_listGiaoHo == value)
                    return;
                _listGiaoHo = value;
                OnPropertyChanged("ListGiaoHo");
            }
        }

        public ObservableCollection<string> ListGioi
        {
            get { return _listGioi; }
            set
            {
                if (_listGioi == value)
                    return;
                _listGioi = value;
                OnPropertyChanged("ListGioi");
            }
        }

        public bool RemoveGiaoHoFilter
        {
            get { return _removeGiaohofilter; }
            set
            {
                _removeGiaohofilter = value;
                OnPropertyChanged("RemoveGiaoHoFilter");
            }
        }

        public bool RemoveGioiFilter
        {
            get { return _removeGioifilter; }
            set
            {
                _removeGioifilter = value;
                OnPropertyChanged("RemoveGioiFilter");
            }
        }

        #endregion

        #region Contructors
        private ObservableCollection<ChiTietGiaoDanViewModel> collection_GiaoDan;

        public DanhSachGiaoDanViewModel()
        {
            Initialize();
            // Requires: using System.Windows.Data
            //ListGiaoDan.Filter += ListGiaoDan_Filter;
        }

        private async void Initialize()
        {
            collection_GiaoDan = new ObservableCollection<ChiTietGiaoDanViewModel>();

            await Task.Run(() =>
            {
                ListGiaoHo = new ObservableCollection<string>(DiaChiServices.GetListGiaoHo());
                ListGioi = new ObservableCollection<string>(new List<string> { "Thiếu Nhi","Giới Trẻ","Gia Trưởng","Hiền Mẫu","Cao Niên" });
                var list = GiaoDanServices.GetAllActiveGiaoDan();
                foreach (var gd in list)
                {
                    var _vm = new ChiTietGiaoDanViewModel();
                    _vm.ID = gd.id;
                    _vm.MaGiaoDan = gd.magiaodan;
                    _vm.TenThanh = gd.tenthanh;
                    _vm.NgaySinh = gd.ngaysinh;
                    _vm.Ho = GiaoXuHelper.SplitLastName(gd.hoten);
                    _vm.Ten = GiaoXuHelper.SplitFirstName(gd.hoten);
                    _vm.GioiTinh = gd.gioitinh;
                    _vm.GiaoHo = gd.giaoho;
                    _vm.Gioi = gd.gioi;
                    _vm.Diachinha = gd.diachinha;
                    _vm.IDCha = gd.cha;
                    _vm.IDMe = gd.me;

                    collection_GiaoDan.Add(_vm);
                }
            });
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                ListGiaoDan = CollectionViewSource.GetDefaultView(collection_GiaoDan);
                ListGiaoDan.SortDescriptions.Add(new SortDescription("Ten", ListSortDirection.Ascending));
            });
            
        }
        #endregion


        #region Commands
        /// <summary>
        /// lưu thông tin
        /// </summary>
        public ICommand AddGiaoDanCommand
        {
            get
            {
                if (_addGiaoDanCommand == null)
                    _addGiaoDanCommand = new AnotherCommandImplementation(AddGiaoDan);
                return _addGiaoDanCommand;
            }
            set
            {
                _addGiaoDanCommand = value;
                OnPropertyChanged("AddGiaoDanCommand");

            }
        }

        private ChiTietGiaoDanViewModel _vm;
        private async void AddGiaoDan(object obj)
        {

            _vm = new ChiTietGiaoDanViewModel();
            //let's set up a little MVVM, cos that's what the cool kids are doing:
            var view = new ThemGiaoDan();
            
            //show the dialog
            var result = await DialogHost.Show(view, "RootDialog", ClosingEventHandler);
            if ((int)result != 0)
            {
                collection_GiaoDan.Add(_vm);
            }
        }

        private void ClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            // add code here
            if ((int)eventArgs.Parameter == 0) return;
            Console.WriteLine("ID la {0}", (int)eventArgs.Parameter);
            var _gd = GiaoDanServices.GetGiaoDanByID((int)eventArgs.Parameter);
            if (_gd != null)
            {
                
                _vm.ID = _gd.id;
                _vm.MaGiaoDan = _gd.magiaodan;
                _vm.TenThanh = _gd.tenthanh;
                _vm.NgaySinh = _gd.ngaysinh;
                _vm.Ho = GiaoXuHelper.SplitLastName(_gd.hoten);
                _vm.Ten = GiaoXuHelper.SplitFirstName(_gd.hoten);
                _vm.GioiTinh = _gd.gioitinh;
                _vm.GiaoHo = _gd.giaoho;
                _vm.Gioi = _gd.gioi;
                _vm.Diachinha = _gd.diachinha;
                _vm.IDCha = _gd.cha;
                _vm.IDMe = _gd.me;

            }
        }
        #endregion

        private CollectionViewSource CVS { get; set; }

        //#region Filter
        //public void AddGiaoHoFilter()
        //{
        //    CVS = ListGiaoDan;
        //    // see Notes on Adding Filters:
        //    if (RemoveGiaoHoFilter)
        //    {
        //        ListGiaoDan.Filter -= new FilterEventHandler(FilterByGiaoHo);
        //        ListGiaoDan.Filter += new FilterEventHandler(FilterByGiaoHo);
        //    }
        //    else
        //    {
        //        ListGiaoDan.Filter += new FilterEventHandler(FilterByGiaoHo);
        //        RemoveGiaoHoFilter = true;
        //    }
        //}
        //public void AddYearFilter()
        //{
        //    // see Notes on Adding Filters:
        //    if (CanRemoveYearFilter)
        //    {
        //        CVS.Filter -= new FilterEventHandler(FilterByYear);
        //        CVS.Filter += new FilterEventHandler(FilterByYear);
        //    }
        //    else
        //    {
        //        CVS.Filter += new FilterEventHandler(FilterByYear);
        //        CanRemoveYearFilter = true;
        //    }
        //}

        //private void FilterByGiaoHo(object sender, FilterEventArgs e)
        //{
        //    // see Notes on Filter Methods:
        //    var src = e.Item as ChiTietGiaoDanViewModel;
        //    if (src == null)
        //        e.Accepted = false;
        //    else if (string.Compare(SelectedGiaoHo, src.GiaoHo) != 0)
        //        e.Accepted = false;
        //}
        //private void FilterByYear(object sender, FilterEventArgs e)
        //{
        //    // see Notes on Filter Methods:
        //    var src = e.Item as Thing;
        //    if (src == null)
        //        e.Accepted = false;
        //    else if (SelectedYear != src.Year)
        //        e.Accepted = false;
        //}

        //private enum FilterField
        //{
        //    GiaoHo,
        //    Gioi,
        //    None
        //}

        //private void ApplyFilter(FilterField field)
        //{
        //    switch(field)
        //    {
        //        case GiaoHo:
        //            break;
        //        case Gioi:
        //            break;
        //        default:
        //            break;

        //    }
        //}
        //#endregion
    }
}
