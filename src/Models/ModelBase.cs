using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModelBase<T> where T : Component
{
    protected readonly T Component;
    public List<Group> Groups { get; set; }
    public ModelBase<T> Mirror { get; set; }
    public IModel MirrorModel => Mirror as IModel;
    public string Id { get; }
    public string Label { get; }
    public bool Selected { get; set; }
    public bool Highlighted { get; set; }

    protected ModelBase(T component, string label)
    {
        if (component == null) throw new ArgumentNullException(nameof(component));
        if (string.IsNullOrEmpty(label)) throw new ArgumentException("message", nameof(label));
        Component = component;
        Groups = new List<Group>();
        Id = component.Uuid();
        Label = label;
    }

    public abstract void SyncPreviews();

    public override string ToString() => Id;
}
