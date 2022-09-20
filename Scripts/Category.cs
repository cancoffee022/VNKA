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

    public static bool operator ==(Category Ihs,string rhs)//string과의 비교연산자
    {
        if (Ihs is null)
            return ReferenceEquals(rhs, null);//둘다 같거나 둘다 null인경우 true
        return Ihs.CodeName == rhs || Ihs.displayName == rhs;
    }

    public static bool operator !=(Category Ihs, string rhs) => !(Ihs == rhs);
    //category.CodeName=="Kill" X
    //category=="Kill" 을 선언했을때 category가 null이면 false 반환, category의 codeName혹은 displayName이 "Kill"이라면 true 
    #endregion
}//Target으로 어떤것을 해야 성공횟수를 받는지
