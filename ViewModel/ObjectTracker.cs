using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace TranslationHelper.ViewModel {
  /// <summary>
  /// ...Shooting myself in the foot...
  /// </summary>
  public sealed class ObjectTracker : IDisposable {
    private readonly IList<INotifyPropertyChanged> _trackedNpcs = new List<INotifyPropertyChanged>(); 
    private readonly IList<ObjectTracker> _trackedChildren = new List<ObjectTracker>();
    private readonly Action _changedCallback;
    private readonly object _source;

    public ObjectTracker( object source, Action changeCallback ) {
      _source = source;
      _changedCallback = changeCallback;
      Track( _source );
    }

    private void Untrack( object obj ) {
      if ( obj == null ) {
        return;
      }
      if ( obj is INotifyPropertyChanged npc ) {
        npc.PropertyChanged -= OnTrackedItemChanged;
        _trackedNpcs.Remove( npc );
      }
      var type = obj.GetType();
      var props = type.GetProperties().Where( prop => prop.CanRead );
      foreach ( var prop in props ) {
        var value = prop.GetValue( obj );
        if ( value is INotifyCollectionChanged childCollectionChanged ) {
          UntrackCollection( childCollectionChanged );
          continue;
        }
        if ( value is INotifyPropertyChanged ) {
          var tracker = _trackedChildren.FirstOrDefault( child => Equals( child._source, value ) );
          if ( tracker == null ) {
            continue;
          }
          tracker.Dispose();
          _trackedChildren.Remove( tracker );
        }
      }
    }

    private void Track( object obj ) {
      if ( obj == null ) {
        return;
      }
      if ( obj is INotifyPropertyChanged npc ) {
        npc.PropertyChanged += OnTrackedItemChanged;
        _trackedNpcs.Add( npc );
      }
      var type = obj.GetType();
      var props = type.GetProperties().Where( prop => prop.CanRead );
      foreach ( var prop in props ) {
        var value = prop.GetValue( obj );
        if ( value is INotifyCollectionChanged childCollectionChanged ) {
          TrackCollection( childCollectionChanged );
          continue;
        }
        if ( value is INotifyPropertyChanged childNpc ) {
          var tracker = new ObjectTracker( childNpc, _changedCallback );
          _trackedChildren.Add( tracker );
        }
      }
    }

    private void TrackCollection( INotifyCollectionChanged collection ) {
      if ( collection == null ) {
        return;
      }
      collection.CollectionChanged += OnCollectionChanged;
      if ( collection is IEnumerable enumerable ) {
        foreach ( var item in enumerable ) {
          Track( item );
        }
      }
    }

    private void OnTrackedItemChanged( object sender, PropertyChangedEventArgs e ) {
      OnTriggerChange();
    }

    private void OnCollectionChanged( object sender, NotifyCollectionChangedEventArgs e ) {
      bool hasChanges = false;
      if ( e.OldItems != null ) {
        foreach ( var item in e.OldItems ) {
          hasChanges = true;
          Untrack( item );
        }
      }
      if ( e.NewItems != null ) {
        foreach ( var item in e.NewItems ) {
          hasChanges = true;
          Track( item );
        }
      }
      if ( hasChanges ) {
        OnTriggerChange();
      }
    }

    private void UntrackCollection( INotifyCollectionChanged collection ) {
      if ( collection == null ) {
        return;
      }
      collection.CollectionChanged -= OnCollectionChanged;
      if ( collection is IEnumerable enumerable ) {
        foreach ( var item in enumerable ) {
          Untrack( item );
        }
      }
    }

    private void OnTriggerChange() {
      _changedCallback?.Invoke();
    }

    public void Dispose() {
      Untrack( _source );
      foreach ( var child in _trackedChildren ) {
        child.Dispose();
      }
      _trackedChildren.Clear();
      foreach ( var npc in _trackedNpcs ) {
        Untrack( npc );
      }
    }
  }
}