using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeWars;

public class RemoveDoubleKeyTests
{
    private List<DoubleKeyModel> models;

    [SetUp]
    public void Initialize()
    {
        // Replace keys in a set with another key using a property name
        var modelData = new List<DoubleKeyModel>
        {
            new DoubleKeyModel("Justin", new Guid("00000001-C2AD-4373-BF50-78DC917CD93B"), 2),
            new DoubleKeyModel("Klaas", new Guid("00000002-C2AD-4373-BF50-78DC917CD93B"), 3),
            new DoubleKeyModel("Justin", new Guid("00000004-C2AD-4373-BF50-78DC917CD93B"), 4),
            new DoubleKeyModel("Henk", new Guid("00000004-C2AD-4373-BF50-78DC917CD93B"), 3)
        };

        models = modelData;
    }

    [Test]
    public void TestReplaceByName()
    {
        // Act
        var updateCount = models.AsQueryable().ReplaceFieldValue(m => m.Name, "Justin", m => m.SubjectId, new Guid("00000001-C2AD-4373-BF50-78DC917CD93B"));

        // Asserts
        Assert.AreEqual(4, models.Count);
        Assert.AreEqual(1, models.Count(m => m.SubjectId == new Guid("00000004-C2AD-4373-BF50-78DC917CD93B")));
        Assert.AreEqual(1, models.Count(m => m.SubjectId == new Guid("00000002-C2AD-4373-BF50-78DC917CD93B")));
        Assert.AreEqual(2, models.Count(m => m.SubjectId == new Guid("00000001-C2AD-4373-BF50-78DC917CD93B")));
        Assert.AreEqual(1, updateCount);
    }

    [Test]
    public void TestReplaceByReference()
    {
        // Act
        var updateCount = models.AsQueryable().ReplaceFieldValue(m => m.Reference,3, m => m.SubjectId, new Guid("00000001-C2AD-4373-BF50-78DC917CD93B"));

        // Asserts
        Assert.AreEqual(4, models.Count);
        Assert.AreEqual(1, models.Count(m => m.SubjectId == new Guid("00000004-C2AD-4373-BF50-78DC917CD93B")));
        Assert.AreEqual(0, models.Count(m => m.SubjectId == new Guid("00000002-C2AD-4373-BF50-78DC917CD93B")));
        Assert.AreEqual(3, models.Count(m => m.SubjectId == new Guid("00000001-C2AD-4373-BF50-78DC917CD93B")));
        Assert.AreEqual(2, updateCount);
    }

    [Test]
    public void TestReplaceBySubjectId()
    {
        // Act
        var updateCount = models.AsQueryable().ReplaceFieldValue(m => m.SubjectId, new Guid("00000004-C2AD-4373-BF50-78DC917CD93B"), m => m.Name, "Henk");

        // Asserts
        Assert.AreEqual(4, models.Count);
        Assert.AreEqual(2, models.Count(m => m.Name == "Henk"));
        Assert.AreEqual(1, models.Count(m => m.Name == "Justin"));
        Assert.AreEqual(1, models.Count(m => m.Name == "Klaas"));
        Assert.AreEqual(1, updateCount);
    }

    [Test]
    public void TestReplaceSubjectIdBySubjectId()
    {
        // Act
        var updateCount = models.AsQueryable().ReplaceFieldValue( m => m.SubjectId, new Guid("00000004-C2AD-4373-BF50-78DC917CD93B"), m => m.SubjectId, new Guid("00000001-C2AD-4373-BF50-78DC917CD93B"));

        // Asserts
        Assert.AreEqual(4, models.Count);
        Assert.AreEqual(0, models.Count(m => m.SubjectId == new Guid("00000004-C2AD-4373-BF50-78DC917CD93B")));
        Assert.AreEqual(1, models.Count(m => m.SubjectId == new Guid("00000002-C2AD-4373-BF50-78DC917CD93B")));
        Assert.AreEqual(3, models.Count(m => m.SubjectId == new Guid("00000001-C2AD-4373-BF50-78DC917CD93B")));
        Assert.AreEqual(2, updateCount);
    }
}

public static class DataSetExtensions { 

    public static int ReplaceFieldValue<T, TMatch, TUpdate>(
        this IQueryable<T> dataSet,
        Expression<Func<T, TMatch>> propertyToMatch, 
        object propertyMatchValue,
        Expression<Func<T, TUpdate>> propertyToModify, 
        object newPropertyValue)
    {
        var countUpdates = 0;
        var pMatch = GetPropertyInfo(propertyToMatch);
        var pUpdate = GetPropertyInfo(propertyToModify);

        var elements = dataSet.Where(GetEqualsLambda<T>(pMatch.Name, propertyMatchValue));
        foreach(var item in elements)
        {
            var oldValue = pUpdate.GetValue(item);
            if (!oldValue.Equals(newPropertyValue))
            {
                pUpdate.SetValue(item, newPropertyValue);
                countUpdates++;
            }
        }
        return countUpdates;
    }

    public static PropertyInfo GetPropertyInfo<TSource, TProperty>(
        Expression<Func<TSource, TProperty>> propertyLambda)
    {
        if (propertyLambda.Body is not MemberExpression member)
        {
            throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a method, not a property.",
                propertyLambda.ToString()));
        }

        if (member.Member is not PropertyInfo propInfo)
        {
            throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a field, not a property.",
                propertyLambda.ToString()));
        }

        Type type = typeof(TSource);
        if (propInfo.ReflectedType != null && type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
        {
            throw new ArgumentException(string.Format(
                "Expression '{0}' refers to a property that is not from type {1}.",
                propertyLambda.ToString(),
                type));
        }

        return propInfo;
    }

    public static Expression<Func<T, bool>> GetEqualsLambda<T>(string propertyName, object filterValue)
    {
        ParameterExpression param = Expression.Parameter(typeof(T));
        Expression exp = EqualsExpression<T>(param, propertyName, filterValue);

        return Expression.Lambda<Func<T, bool>>(exp, param);
    }

    private static Expression EqualsExpression<T>(ParameterExpression param, string propertyName, object filterValue)
    {
        MemberExpression evaluateMember = Expression.Property(param, propertyName);
        ConstantExpression constant = Expression.Constant(filterValue);
        return Expression.Equal(evaluateMember, constant);
    }
}

    public class DoubleKeyModel
{
    public DoubleKeyModel(string name, Guid subjectId, int reference)
    {
        Name=name; 
        SubjectId=subjectId; 
        Reference=reference;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Guid SubjectId { get; set; } = Guid.Empty;
    public int Reference { get; set; }
}