﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PetersWeb
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="Peter")]
	public partial class DBContextDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertPage(Page instance);
    partial void UpdatePage(Page instance);
    partial void DeletePage(Page instance);
    partial void InsertTermToPage(TermToPage instance);
    partial void UpdateTermToPage(TermToPage instance);
    partial void DeleteTermToPage(TermToPage instance);
    partial void InsertTerm(Term instance);
    partial void UpdateTerm(Term instance);
    partial void DeleteTerm(Term instance);
    partial void InsertShingle(Shingle instance);
    partial void UpdateShingle(Shingle instance);
    partial void DeleteShingle(Shingle instance);
    #endregion
		
		public DBContextDataContext() : 
				base(global::PetersWeb.Properties.Settings.Default.PeterConnectionString1, mappingSource)
		{
			OnCreated();
		}
		
		public DBContextDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DBContextDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DBContextDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public DBContextDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Page> Pages
		{
			get
			{
				return this.GetTable<Page>();
			}
		}
		
		public System.Data.Linq.Table<TermToPage> TermToPages
		{
			get
			{
				return this.GetTable<TermToPage>();
			}
		}
		
		public System.Data.Linq.Table<Term> Terms
		{
			get
			{
				return this.GetTable<Term>();
			}
		}
		
		public System.Data.Linq.Table<Shingle> Shingles
		{
			get
			{
				return this.GetTable<Shingle>();
			}
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.DeleteAllPages")]
		public int DeleteAllPages()
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())));
			return ((int)(result.ReturnValue));
		}
		
		[global::System.Data.Linq.Mapping.FunctionAttribute(Name="dbo.insertToken")]
		public int insertToken([global::System.Data.Linq.Mapping.ParameterAttribute(DbType="VarChar(MAX)")] string term, [global::System.Data.Linq.Mapping.ParameterAttribute(DbType="VarChar(MAX)")] string url)
		{
			IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), term, url);
			return ((int)(result.ReturnValue));
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Pages")]
	public partial class Page : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _id;
		
		private string _url;
		
		private string _html;
		
		private System.Data.Linq.Binary _timestamp;
		
		private EntitySet<TermToPage> _TermToPages;
		
		private EntitySet<Shingle> _Shingles;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnidChanging(int value);
    partial void OnidChanged();
    partial void OnurlChanging(string value);
    partial void OnurlChanged();
    partial void OnhtmlChanging(string value);
    partial void OnhtmlChanged();
    partial void OntimestampChanging(System.Data.Linq.Binary value);
    partial void OntimestampChanged();
    #endregion
		
		public Page()
		{
			this._TermToPages = new EntitySet<TermToPage>(new Action<TermToPage>(this.attach_TermToPages), new Action<TermToPage>(this.detach_TermToPages));
			this._Shingles = new EntitySet<Shingle>(new Action<Shingle>(this.attach_Shingles), new Action<Shingle>(this.detach_Shingles));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true, UpdateCheck=UpdateCheck.Never)]
		public int id
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((this._id != value))
				{
					this.OnidChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("id");
					this.OnidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_url", DbType="VarChar(MAX) NOT NULL", CanBeNull=false, UpdateCheck=UpdateCheck.Never)]
		public string url
		{
			get
			{
				return this._url;
			}
			set
			{
				if ((this._url != value))
				{
					this.OnurlChanging(value);
					this.SendPropertyChanging();
					this._url = value;
					this.SendPropertyChanged("url");
					this.OnurlChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_html", DbType="VarChar(MAX)", UpdateCheck=UpdateCheck.Never)]
		public string html
		{
			get
			{
				return this._html;
			}
			set
			{
				if ((this._html != value))
				{
					this.OnhtmlChanging(value);
					this.SendPropertyChanging();
					this._html = value;
					this.SendPropertyChanged("html");
					this.OnhtmlChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_timestamp", AutoSync=AutoSync.Always, DbType="rowversion NOT NULL", CanBeNull=false, IsDbGenerated=true, IsVersion=true, UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary timestamp
		{
			get
			{
				return this._timestamp;
			}
			set
			{
				if ((this._timestamp != value))
				{
					this.OntimestampChanging(value);
					this.SendPropertyChanging();
					this._timestamp = value;
					this.SendPropertyChanged("timestamp");
					this.OntimestampChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Page_TermToPage", Storage="_TermToPages", ThisKey="id", OtherKey="pageID")]
		public EntitySet<TermToPage> TermToPages
		{
			get
			{
				return this._TermToPages;
			}
			set
			{
				this._TermToPages.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Page_Shingle", Storage="_Shingles", ThisKey="id", OtherKey="url")]
		public EntitySet<Shingle> Shingles
		{
			get
			{
				return this._Shingles;
			}
			set
			{
				this._Shingles.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_TermToPages(TermToPage entity)
		{
			this.SendPropertyChanging();
			entity.Page = this;
		}
		
		private void detach_TermToPages(TermToPage entity)
		{
			this.SendPropertyChanging();
			entity.Page = null;
		}
		
		private void attach_Shingles(Shingle entity)
		{
			this.SendPropertyChanging();
			entity.Page = this;
		}
		
		private void detach_Shingles(Shingle entity)
		{
			this.SendPropertyChanging();
			entity.Page = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.TermToPage")]
	public partial class TermToPage : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _termID;
		
		private int _pageID;
		
		private System.Nullable<int> _count;
		
		private EntityRef<Page> _Page;
		
		private EntityRef<Term> _Term;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OntermIDChanging(int value);
    partial void OntermIDChanged();
    partial void OnpageIDChanging(int value);
    partial void OnpageIDChanged();
    partial void OncountChanging(System.Nullable<int> value);
    partial void OncountChanged();
    #endregion
		
		public TermToPage()
		{
			this._Page = default(EntityRef<Page>);
			this._Term = default(EntityRef<Term>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_termID", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int termID
		{
			get
			{
				return this._termID;
			}
			set
			{
				if ((this._termID != value))
				{
					if (this._Term.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OntermIDChanging(value);
					this.SendPropertyChanging();
					this._termID = value;
					this.SendPropertyChanged("termID");
					this.OntermIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_pageID", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int pageID
		{
			get
			{
				return this._pageID;
			}
			set
			{
				if ((this._pageID != value))
				{
					if (this._Page.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnpageIDChanging(value);
					this.SendPropertyChanging();
					this._pageID = value;
					this.SendPropertyChanged("pageID");
					this.OnpageIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_count", DbType="Int")]
		public System.Nullable<int> count
		{
			get
			{
				return this._count;
			}
			set
			{
				if ((this._count != value))
				{
					this.OncountChanging(value);
					this.SendPropertyChanging();
					this._count = value;
					this.SendPropertyChanged("count");
					this.OncountChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Page_TermToPage", Storage="_Page", ThisKey="pageID", OtherKey="id", IsForeignKey=true)]
		public Page Page
		{
			get
			{
				return this._Page.Entity;
			}
			set
			{
				Page previousValue = this._Page.Entity;
				if (((previousValue != value) 
							|| (this._Page.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Page.Entity = null;
						previousValue.TermToPages.Remove(this);
					}
					this._Page.Entity = value;
					if ((value != null))
					{
						value.TermToPages.Add(this);
						this._pageID = value.id;
					}
					else
					{
						this._pageID = default(int);
					}
					this.SendPropertyChanged("Page");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Term_TermToPage", Storage="_Term", ThisKey="termID", OtherKey="id", IsForeignKey=true)]
		public Term Term
		{
			get
			{
				return this._Term.Entity;
			}
			set
			{
				Term previousValue = this._Term.Entity;
				if (((previousValue != value) 
							|| (this._Term.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Term.Entity = null;
						previousValue.TermToPages.Remove(this);
					}
					this._Term.Entity = value;
					if ((value != null))
					{
						value.TermToPages.Add(this);
						this._termID = value.id;
					}
					else
					{
						this._termID = default(int);
					}
					this.SendPropertyChanged("Term");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Terms")]
	public partial class Term : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _id;
		
		private string _term1;
		
		private EntitySet<TermToPage> _TermToPages;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnidChanging(int value);
    partial void OnidChanged();
    partial void Onterm1Changing(string value);
    partial void Onterm1Changed();
    #endregion
		
		public Term()
		{
			this._TermToPages = new EntitySet<TermToPage>(new Action<TermToPage>(this.attach_TermToPages), new Action<TermToPage>(this.detach_TermToPages));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int id
		{
			get
			{
				return this._id;
			}
			set
			{
				if ((this._id != value))
				{
					this.OnidChanging(value);
					this.SendPropertyChanging();
					this._id = value;
					this.SendPropertyChanged("id");
					this.OnidChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="term", Storage="_term1", DbType="VarChar(MAX) NOT NULL", CanBeNull=false)]
		public string term1
		{
			get
			{
				return this._term1;
			}
			set
			{
				if ((this._term1 != value))
				{
					this.Onterm1Changing(value);
					this.SendPropertyChanging();
					this._term1 = value;
					this.SendPropertyChanged("term1");
					this.Onterm1Changed();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Term_TermToPage", Storage="_TermToPages", ThisKey="id", OtherKey="termID")]
		public EntitySet<TermToPage> TermToPages
		{
			get
			{
				return this._TermToPages;
			}
			set
			{
				this._TermToPages.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_TermToPages(TermToPage entity)
		{
			this.SendPropertyChanging();
			entity.Term = this;
		}
		
		private void detach_TermToPages(TermToPage entity)
		{
			this.SendPropertyChanging();
			entity.Term = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Shingles")]
	public partial class Shingle : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _url;
		
		private int _shingle1;
		
		private EntityRef<Page> _Page;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnurlChanging(int value);
    partial void OnurlChanged();
    partial void Onshingle1Changing(int value);
    partial void Onshingle1Changed();
    #endregion
		
		public Shingle()
		{
			this._Page = default(EntityRef<Page>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_url", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int url
		{
			get
			{
				return this._url;
			}
			set
			{
				if ((this._url != value))
				{
					if (this._Page.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnurlChanging(value);
					this.SendPropertyChanging();
					this._url = value;
					this.SendPropertyChanged("url");
					this.OnurlChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Name="shingle", Storage="_shingle1", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int shingle1
		{
			get
			{
				return this._shingle1;
			}
			set
			{
				if ((this._shingle1 != value))
				{
					this.Onshingle1Changing(value);
					this.SendPropertyChanging();
					this._shingle1 = value;
					this.SendPropertyChanged("shingle1");
					this.Onshingle1Changed();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Page_Shingle", Storage="_Page", ThisKey="url", OtherKey="id", IsForeignKey=true)]
		public Page Page
		{
			get
			{
				return this._Page.Entity;
			}
			set
			{
				Page previousValue = this._Page.Entity;
				if (((previousValue != value) 
							|| (this._Page.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Page.Entity = null;
						previousValue.Shingles.Remove(this);
					}
					this._Page.Entity = value;
					if ((value != null))
					{
						value.Shingles.Add(this);
						this._url = value.id;
					}
					else
					{
						this._url = default(int);
					}
					this.SendPropertyChanged("Page");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
