using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TranslationHelper.Behaviors {
  public static class GridViewColumns {
    [AttachedPropertyBrowsableForType( typeof( GridView ) )]
    public static object GetColumnsSource( DependencyObject obj ) {
      return (object)obj.GetValue( ColumnsSourceProperty );
    }

    public static void SetColumnsSource( DependencyObject obj, object value ) {
      obj.SetValue( ColumnsSourceProperty, value );
    }

    // Using a DependencyProperty as the backing store for ColumnsSource.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ColumnsSourceProperty =
      DependencyProperty.RegisterAttached(
        "ColumnsSource",
        typeof( object ),
        typeof( GridViewColumns ),
        new UIPropertyMetadata(
          null,
          ColumnsSourceChanged ) );

    private static void ColumnsSourceChanged( DependencyObject obj, DependencyPropertyChangedEventArgs e ) {
      GridView gridView = obj as GridView;
      if ( gridView != null ) {
        gridView.Columns.Clear();

        if ( e.OldValue != null ) {
          ICollectionView view = CollectionViewSource.GetDefaultView( e.OldValue );
          if ( view != null )
            RemoveHandlers( gridView, view );
        }

        if ( e.NewValue != null ) {
          ICollectionView view = CollectionViewSource.GetDefaultView( e.NewValue );
          if ( view != null ) {
            AddHandlers( gridView, view );
            CreateColumns( gridView, view );
          }
        }
      }
    }

    private static readonly IDictionary<ICollectionView, List<GridView>> GridViewsByColumnsSource =
      new Dictionary<ICollectionView, List<GridView>>();

    private static List<GridView> GetGridViewsForColumnSource( ICollectionView columnSource ) {
      List<GridView> gridViews;
      if ( !GridViewsByColumnsSource.TryGetValue( columnSource, out gridViews ) ) {
        gridViews = new List<GridView>();
        GridViewsByColumnsSource.Add( columnSource, gridViews );
      }
      return gridViews;
    }

    private static void AddHandlers( GridView gridView, ICollectionView view ) {
      GetGridViewsForColumnSource( view ).Add( gridView );
      view.CollectionChanged += ColumnsSource_CollectionChanged;
    }

    private static void CreateColumns( GridView gridView, ICollectionView view ) {
      foreach ( var item in view ) {
        if ( item is GridViewColumn gvc ) {
          var column = new GridViewColumn {
            Header = gvc.Header,
            DisplayMemberBinding = gvc.DisplayMemberBinding
          };
          gridView.Columns.Add( column );
          continue;
        }
        gridView.Columns.Add( CreateColumn( gridView, item ) );
      }
    }

    private static void RemoveHandlers( GridView gridView, ICollectionView view ) {
      view.CollectionChanged -= ColumnsSource_CollectionChanged;
      GetGridViewsForColumnSource( view ).Remove( gridView );
    }

    private static void ColumnsSource_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e ) {
      ICollectionView view = sender as ICollectionView;
      var gridViews = GetGridViewsForColumnSource( view );
      if ( gridViews == null || gridViews.Count == 0 )
        return;

      switch ( e.Action ) {
        case NotifyCollectionChangedAction.Add:
          foreach ( var gridView in gridViews ) {
            for ( int i = 0; i < e.NewItems.Count; i++ ) {
              GridViewColumn column = CreateColumn( gridView, e.NewItems[i] );
              gridView.Columns.Insert( e.NewStartingIndex + i, column );
            }
          }
          break;
        case NotifyCollectionChangedAction.Move:
          foreach ( var gridView in gridViews ) {
            List<GridViewColumn> columns = new List<GridViewColumn>();
            for ( int i = 0; i < e.OldItems.Count; i++ ) {
              GridViewColumn column = gridView.Columns[e.OldStartingIndex + i];
              columns.Add( column );
            }
            for ( int i = 0; i < e.NewItems.Count; i++ ) {
              GridViewColumn column = columns[i];
              gridView.Columns.Insert( e.NewStartingIndex + i, column );
            }
          }
          break;
        case NotifyCollectionChangedAction.Remove:
          foreach ( var gridView in gridViews ) {
            for ( int i = 0; i < e.OldItems.Count; i++ ) {
              gridView.Columns.RemoveAt( e.OldStartingIndex );
            }
          }
          break;
        case NotifyCollectionChangedAction.Replace:
          foreach ( var gridView in gridViews ) {
            for ( int i = 0; i < e.NewItems.Count; i++ ) {
              GridViewColumn column = CreateColumn( gridView, e.NewItems[i] );
              gridView.Columns[e.NewStartingIndex + i] = column;
            }
          }
          break;
        case NotifyCollectionChangedAction.Reset:
          foreach ( var gridView in gridViews ) {
            gridView.Columns.Clear();
            CreateColumns( gridView, sender as ICollectionView );
          }
          break;
      }
    }

    private static GridViewColumn CreateColumn( GridView gridView, object columnSource ) {
      if ( columnSource is GridViewColumn existingColumn ) {
        return new GridViewColumn {
          Header = existingColumn.Header,
          DisplayMemberBinding = existingColumn.DisplayMemberBinding
        };
      }
      GridViewColumn column = new GridViewColumn();
      return column;
    }
  }
}