using System;
using UnityEngine;

namespace TFs.Common.Entities
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class UpdateMethodAttribute : Attribute
    {
        public UpdateType UpdateType { get; }

        public UpdateMethodAttribute(UpdateType updateType)
        {
            UpdateType = updateType;
        }
    }

    public enum UpdateType
    {
        Update,
        FixedUpdate,
        LateUpdate
    }    
}