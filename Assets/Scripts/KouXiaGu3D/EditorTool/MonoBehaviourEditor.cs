﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace KouXiaGu
{

#if UNITY_EDITOR

    using UnityEditor;

    /// <summary>
    /// 编辑器类,用于重写 MonoBehaviour 的编辑器(仅在存在 CustomEditorAttribute 特性的类上有效);
    /// </summary>
    [@CustomEditor(typeof(MonoBehaviour), true)]
    public class MonoBehaviourEditor : Editor
    {
        protected MonoBehaviourEditor() { }

        PropertyEditorAttribute.Property[] properties;

        public void OnEnable()
        {
            var editorClass = Attribute.GetCustomAttribute(target.GetType(), typeof(CustomEditorAttribute));

            if (editorClass != null)
            {
                properties = PropertyEditorAttribute.GetProperties(target).ToArray();
            }
            //Debug.Log(isEditorClass + " " + target.GetType().Name + " " + properties.Length);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            PropertyGUI();
        }

        /// <summary>
        /// PropertyEditorAttribute
        /// </summary>
        public void PropertyGUI()
        {
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    property.OnGUI();
                }
            }
        }
    }
#endif



    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class CustomEditorAttribute : Attribute
    {

    }

    /// <summary>
    /// 编辑器中属性获取抽象类;
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class PropertyEditorAttribute : Attribute
    {
        public PropertyEditorAttribute()
        {
            this.Label = string.Empty;
        }
        public PropertyEditorAttribute(string label)
        {
            this.Label = label;
        }

        private const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Public |
            BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty;

        /// <summary>
        /// 显示的标签;
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// 渲染到检视面板的方法;
        /// </summary>
        public abstract void OnGUI(Property property);

        /// <summary>
        /// 获取到所有添加此特性的可读或可写的属性;
        /// </summary>
        public static IEnumerable<Property> GetProperties(object instance, BindingFlags bindingAttr = DefaultBindingFlags)
        {
            PropertyInfo[] propertyInfos = instance.GetType().GetProperties(bindingAttr);

            foreach (var propertyInfo in propertyInfos)
            {
                if (!propertyInfo.CanRead && !propertyInfo.CanWrite)
                    continue;

                var attribute = (PropertyEditorAttribute)GetCustomAttribute(propertyInfo, typeof(PropertyEditorAttribute));
                if (attribute != null)
                {
                    Property property = new Property(instance, propertyInfo, attribute);
                    yield return property;
                }
            }
        }

        protected virtual string GetLabel(PropertyInfo propertyInfo, MethodInfo getter, MethodInfo setter)
        {
            string label = this.Label == string.Empty ? propertyInfo.Name : this.Label;
            return label;
        }

        public class Property
        {
            public Property(object instance, PropertyInfo propertyInfo, PropertyEditorAttribute attribute)
            {
                this.Instance = instance;
                this.attribute = attribute;
                this.propertyInfo = propertyInfo;

                this.getter = propertyInfo.GetGetMethod(true);
                this.setter = propertyInfo.GetSetMethod(true);
                this.ObjectType = propertyInfo.PropertyType.GetObjectType();
                this.Label = attribute.GetLabel(propertyInfo, this.getter, this.setter);
            }

            private PropertyEditorAttribute attribute;
            private MethodInfo getter;
            private MethodInfo setter;
            private PropertyInfo propertyInfo;

            public ObjectType ObjectType { get; private set; }
            public object Instance { get; private set; }
            public string Label { get; private set; }
            public Type PropertyInfo { get { return propertyInfo.PropertyType; } }
            public bool CanRead { get { return getter != null; } }
            public bool CanWrite { get { return setter != null; } }

            public object Value
            {
                get
                {
                    return CanRead ? getter.Invoke(Instance, null) : null;
                }
                set
                {
                    if (CanWrite)
                        setter.Invoke(Instance, new object[] { value });
                }
            }

            /// <summary>
            /// 渲染到检视面板的方法;
            /// </summary>
            public void OnGUI()
            {
                attribute.OnGUI(this);
            }
        }
    }


    /// <summary>
    /// 在检视面板显示这个属性;
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ShowOnlyPropertyAttribute : PropertyEditorAttribute
    {
        public ShowOnlyPropertyAttribute() : base() { }
        public ShowOnlyPropertyAttribute(string label) : base(label) { }

        public override void OnGUI(Property property)
        {
#if UNITY_EDITOR
            try
            {
                EditorGUILayout.LabelField(property.Label, property.Value.ToString());
            }
            catch
            {
                EditorGUILayout.LabelField(property.Label, "未能获取");
            }
#endif
        }

    }


    /// <summary>
    /// 在检视面板暴露这个属性,若不可读则显示为标签,否则显示为可编辑状态;
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ExposePropertyAttribute : PropertyEditorAttribute
    {
        public ExposePropertyAttribute() : base() { }
        public ExposePropertyAttribute(string label) : base(label) { }

        private const string UnbeknownTypeLabel = "Unbeknown";

        public override void OnGUI(Property property)
        {
#if UNITY_EDITOR
            try
            {
                if (!property.CanWrite)
                {
                    EditorGUILayout.LabelField(property.Label, property.Value.ToString());
                    return;
                }

                switch (property.ObjectType)
                {
                    case ObjectType.Integer:
                        property.Value = EditorGUILayout.IntField(property.Label, (int)property.Value);
                        break;
                    case ObjectType.Float:
                        property.Value = EditorGUILayout.FloatField(property.Label, (float)property.Value);
                        break;
                    case ObjectType.Boolean:
                        property.Value = EditorGUILayout.Toggle(property.Label, (bool)property.Value);
                        break;
                    case ObjectType.String:
                        property.Value = EditorGUILayout.TextField(property.Label, (string)property.Value);
                        break;
                    case ObjectType.Vector2:
                        property.Value = EditorGUILayout.Vector2Field(property.Label, (Vector2)property.Value);
                        break;
                    case ObjectType.Vector3:
                        property.Value = EditorGUILayout.Vector3Field(property.Label, (Vector3)property.Value);
                        break;
                    case ObjectType.Enum:
                        property.Value = EditorGUILayout.EnumPopup(property.Label, (Enum)property.Value);
                        break;
                    case ObjectType.ObjectReference:
                        property.Value = EditorGUILayout.ObjectField(property.Label, (UnityEngine.Object)property.Value, property.PropertyInfo, true);
                        break;
                    default:
                        EditorGUILayout.LabelField(UnbeknownTypeLabel, property.Value.ToString());
                        break;
                }
            }
            catch
            {
                EditorGUILayout.LabelField(property.Label, "未能获取");
            }
#endif
        }
        
    }

}