namespace DatabaseNormalization

open System.Collections.Generic

// 基础类型定义
type AttributeName = string
type RelationName = string

type CellValue =
    | Null
    | Int of int
    | String of string
    | Bool of bool
    | Float of float
    | Date of System.DateTime

    interface System.IComparable with
        member this.CompareTo(other) =
            match other with
            | :? CellValue as o ->
                match this, o with
                | Null, Null -> 0
                | Null, _ -> -1
                | _, Null -> 1
                | Int a, Int b -> compare a b
                | String a, String b -> compare a b
                | Bool a, Bool b -> compare a b
                | Float a, Float b -> compare a b
                | Date a, Date b -> compare a b
                | _ -> 0
            | _ -> -1

type Domain = Set<CellValue>
type AttributeSet = Set<AttributeName>
type Tuple = Map<AttributeName, CellValue>

// 函数依赖 (Functional Dependency)
type FunctionalDependency = {
    Left: AttributeSet      // X
    Right: AttributeSet     // Y
    // 表示 X -> Y
}

// 多值依赖 (Multivalued Dependency)
type MultivaluedDependency = {
    Left: AttributeSet      // X
    Right: AttributeSet     // Y
    // 表示 X ->-> Y
}

// 连接依赖 (Join Dependency)
type JoinDependency = {
    Decomposition: AttributeSet list
}

// 基础关系
type Relation = {
    Name: RelationName
    Attributes: AttributeName list
    Tuples: Tuple list
}

// 1NF: First Normal Form
type Relation1NF = {
    Relation: Relation
    // 所有属性值必须是原子的（不可再分）
}

module Relation1NF =
    let isAtomic (value: CellValue) =
        // 在我们的定义中，所有 CellValue 都是原子的
        match value with
        | Null | Int _ | String _ | Bool _ | Float _ | Date _ -> true

    let validate (relation: Relation) : Relation1NF option =
        let allAtomic =
            relation.Tuples
            |> List.forall (fun tuple ->
                tuple |> Map.forall (fun _ v -> isAtomic v)
            )
        if allAtomic then Some { Relation = relation }
        else None

// 2NF: Second Normal Form
type Relation2NF = {
    Relation1NF: Relation1NF
    PrimaryKey: AttributeSet
    FunctionalDependencies: FunctionalDependency list
    // 没有部分依赖：所有非主属性完全依赖于候选键
}

module Relation2NF =
    let isProperSubset (subset: AttributeSet) (superset: AttributeSet) =
        Set.isSubset subset superset && subset <> superset

    let hasPartialDependency (primaryKey: AttributeSet) (fd: FunctionalDependency) (nonPrimeAttrs: AttributeSet) =
        // 检查是否存在部分依赖
        // 即：fd.Left 是主键的真子集，且 fd.Right 包含非主属性
        isProperSubset fd.Left primaryKey &&
        not (Set.isEmpty (Set.intersect fd.Right nonPrimeAttrs))

    let validate (relation1nf: Relation1NF) (primaryKey: AttributeSet) (fds: FunctionalDependency list) =
        let allAttrs = relation1nf.Relation.Attributes |> Set.ofList
        let nonPrimeAttrs = Set.difference allAttrs primaryKey

        let hasPartialDeps =
            fds |> List.exists (fun fd ->
                hasPartialDependency primaryKey fd nonPrimeAttrs
            )

        if not hasPartialDeps then
            Some {
                Relation1NF = relation1nf
                PrimaryKey = primaryKey
                FunctionalDependencies = fds
            }
        else None

// 3NF: Third Normal Form
type Relation3NF = {
    Relation2NF: Relation2NF
    CandidateKeys: AttributeSet list
    // 没有传递依赖：非主属性不依赖于其他非主属性
}

module Relation3NF =
    let isSuperkey (relation: Relation) (candidateKeys: AttributeSet list) (attrs: AttributeSet) =
        candidateKeys |> List.exists (fun key -> Set.isSubset key attrs)

    let isPrimeAttribute (candidateKeys: AttributeSet list) (attr: AttributeName) =
        candidateKeys |> List.exists (fun key -> Set.contains attr key)

    let violates3NF (candidateKeys: AttributeSet list) (fd: FunctionalDependency) (allAttrs: AttributeSet) =
        // 违反3NF如果：
        // 1. X 不是超键
        // 2. Y 包含非主属性
        let relation = { Name = ""; Attributes = Set.toList allAttrs; Tuples = [] }
        let isSuper = isSuperkey relation candidateKeys fd.Left

        if isSuper then false
        else
            // 检查 fd.Right 是否包含非主属性
            fd.Right |> Set.exists (fun attr -> not (isPrimeAttribute candidateKeys attr))

    let validate (relation2nf: Relation2NF) (candidateKeys: AttributeSet list) =
        let allAttrs = relation2nf.Relation1NF.Relation.Attributes |> Set.ofList

        let violations =
            relation2nf.FunctionalDependencies
            |> List.exists (fun fd -> violates3NF candidateKeys fd allAttrs)

        if not violations then
            Some {
                Relation2NF = relation2nf
                CandidateKeys = candidateKeys
            }
        else None

// BCNF: Boyce-Codd Normal Form
type RelationBCNF = {
    Relation3NF: Relation3NF
    // 更严格：对于每个非平凡函数依赖 X -> Y，X 必须是超键
}

module RelationBCNF =
    let isTrivialDependency (fd: FunctionalDependency) =
        Set.isSubset fd.Right fd.Left

    let isSuperkey (relation: Relation) (candidateKeys: AttributeSet list) (attrs: AttributeSet) =
        candidateKeys |> List.exists (fun key -> Set.isSubset key attrs)

    let validate (relation3nf: Relation3NF) =
        let relation = relation3nf.Relation2NF.Relation1NF.Relation
        let candidateKeys = relation3nf.CandidateKeys

        let allValid =
            relation3nf.Relation2NF.FunctionalDependencies
            |> List.forall (fun fd ->
                isTrivialDependency fd ||
                isSuperkey relation candidateKeys fd.Left
            )

        if allValid then
            Some { Relation3NF = relation3nf }
        else None

// 4NF: Fourth Normal Form
type Relation4NF = {
    RelationBCNF: RelationBCNF
    MultivaluedDependencies: MultivaluedDependency list
    // 对于每个非平凡多值依赖 X ->-> Y，X 必须是超键
}

module Relation4NF =
    let isTrivialMVD (mvd: MultivaluedDependency) (allAttrs: AttributeSet) =
        Set.isEmpty mvd.Right ||
        Set.isSubset allAttrs (Set.union mvd.Left mvd.Right)

    let validate (bcnf: RelationBCNF) (mvds: MultivaluedDependency list) =
        let relation = bcnf.Relation3NF.Relation2NF.Relation1NF.Relation
        let allAttrs = relation.Attributes |> Set.ofList
        let candidateKeys = bcnf.Relation3NF.CandidateKeys

        let allValid =
            mvds |> List.forall (fun mvd ->
                isTrivialMVD mvd allAttrs ||
                RelationBCNF.isSuperkey relation candidateKeys mvd.Left
            )

        if allValid then
            Some {
                RelationBCNF = bcnf
                MultivaluedDependencies = mvds
            }
        else None

// 5NF (PJNF): Fifth Normal Form / Project-Join Normal Form
type Relation5NF = {
    Relation4NF: Relation4NF
    JoinDependencies: JoinDependency list
    // 每个连接依赖都由候选键所蕴含
}

module Relation5NF =
    let validate (r4nf: Relation4NF) (jds: JoinDependency list) =
        // 简化验证：实际实现需要检查连接依赖是否由候选键蕴含
        Some {
            Relation4NF = r4nf
            JoinDependencies = jds
        }

// 6NF: Sixth Normal Form
type Relation6NF = {
    Relation5NF: Relation5NF
    // 支持所有连接依赖（最终范式，完全分解）
}

// DKNF: Domain-Key Normal Form
type Constraint =
    | DomainConstraint of AttributeName * Domain
    | KeyConstraint of AttributeSet

type RelationDKNF = {
    Relation: Relation
    Constraints: Constraint list
    // 所有约束都是域约束或键约束的逻辑结果
}
