using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName ="Category",fileName ="Category_")]
public class Category : ScriptableObject,IEquatable<Category>
{
    [SerializeField]
    private string codeName;
    [SerializeField]
    private string displayName;

    public string CodeName => codeName;
    public string DisPlayName => displayName;

    #region Operator
    public bool Equals(Category other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(other, this))
            return true;
        if (GetType() != other.GetType())
            return false;

        return codeName == other.codeName;
    }

    public override int GetHashCode() => (codeName, DisPlayName).GetHashCode();
    public override bool Equals(object other) => base.Equals(other);

    public static bool operator ==(Category Ihs,string rhs)//string���� �񱳿�����
    {
        if (Ihs is null)
            return ReferenceEquals(rhs, null);//�Ѵ� ���ų� �Ѵ� null�ΰ�� true
        return Ihs.CodeName == rhs || Ihs.displayName == rhs;
    }

    public static bool operator !=(Category Ihs, string rhs) => !(Ihs == rhs);
    //category.CodeName=="Kill" X
    //category=="Kill" �� ���������� category�� null�̸� false ��ȯ, category�� codeNameȤ�� displayName�� "Kill"�̶�� true 
    #endregion
}//Target���� ����� �ؾ� ����Ƚ���� �޴���
