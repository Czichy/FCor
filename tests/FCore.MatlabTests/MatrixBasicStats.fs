﻿namespace FCore.MatlabTests

open FCore
open FCore.Math
open FCore.BasicStats
open FCore.Random
open Xunit
open FsUnit
open FsUnit.Xunit
open FsCheck
open FsCheck.Xunit
open System
open MLApp
open Util

module MatrixBasicStats =

    let app = new MLAppClass()
    do app.Visible <- 0

    let rng = new MT19937Rng()


    let inline (<=>) (x : float[]) (y :float[]) = epsEqualArray x y epsEqualFloat 0.0

    let inline epsEqual eps (x : float[]) (y :float[])  = epsEqualArray x y epsEqualFloat eps

    let inline epsEqual2D eps (x : float[,]) (y :float[,])  = epsEqualArray2D x y epsEqualFloat eps

    let axisNum (axis :  MatrixAxis) =
        match axis with | ColumnAxis -> 1.0 | RowAxis -> 2.0

    [<Property>]
    let ``sum``(v : float[,]) (axis : MatrixAxis) =
        let v = new Matrix(v)
        match axis with
            | ColumnAxis -> setMatrix app "v" (v.ToArray2D())
            | RowAxis -> setMatrix app "v" (transpose(v).ToArray2D())
        app.Execute("res = sum(v);") |> ignore
        let res = getVector app "res"
        ((v.RowCount > 1 && v.ColCount > 1) ==> lazy(epsEqual 1e-15 ((sum v axis).ToArray()) res))

    [<Property>]
    let ``prod``(v : float[,]) (axis : MatrixAxis) =
        let v = new Matrix(v)
        match axis with
            | ColumnAxis -> setMatrix app "v" (v.ToArray2D())
            | RowAxis -> setMatrix app "v" (transpose(v).ToArray2D())
        app.Execute("res = prod(v);") |> ignore
        let res = getVector app "res"
        ((v.RowCount > 1 && v.ColCount > 1) ==> lazy(epsEqual 1e-15 ((prod v axis).ToArray()) res))

    [<Property>]
    let ``cumsum``(v : float[,]) (axis : MatrixAxis) =
        let v = new Matrix(v)
        match axis with
            | ColumnAxis -> setMatrix app "v" (v.ToArray2D())
            | RowAxis -> setMatrix app "v" (transpose(v).ToArray2D())
        app.Execute("res = cumsum(v);") |> ignore
        let res = new Matrix(getMatrix app "res")
        if axis = RowAxis then res.Transpose()
        ((v.RowCount > 1 && v.ColCount > 1) ==> lazy(epsEqual2D 1e-14 ((cumsum v axis).ToArray2D()) (res.ToArray2D())))

    [<Property>]
    let ``cumprod``(v : float[,]) (axis : MatrixAxis) =
        let v = new Matrix(v)
        match axis with
            | ColumnAxis -> setMatrix app "v" (v.ToArray2D())
            | RowAxis -> setMatrix app "v" (transpose(v).ToArray2D())
        app.Execute("res = cumprod(v);") |> ignore
        let res = new Matrix(getMatrix app "res")
        if axis = RowAxis then res.Transpose()
        ((v.RowCount > 1 && v.ColCount > 1) ==> lazy(epsEqual2D 1e-15 ((cumprod v axis).ToArray2D()) (res.ToArray2D())))

    [<Property>]
    let ``min``(v : float[,]) (axis : MatrixAxis) =
        let v = new Matrix(v)
        match axis with
            | ColumnAxis -> setMatrix app "v" (v.ToArray2D())
            | RowAxis -> setMatrix app "v" (transpose(v).ToArray2D())
        app.Execute("res = min(v);") |> ignore
        let res = getVector app "res"
        ((v.RowCount > 1 && v.ColCount > 1) ==> lazy(epsEqual 1e-15 ((min v axis).ToArray()) res))

    [<Property>]
    let ``max``(v : float[,]) (axis : MatrixAxis) =
        let v = new Matrix(v)
        match axis with
            | ColumnAxis -> setMatrix app "v" (v.ToArray2D())
            | RowAxis -> setMatrix app "v" (transpose(v).ToArray2D())
        app.Execute("res = max(v);") |> ignore
        let res = getVector app "res"
        ((v.RowCount > 1 && v.ColCount > 1) ==> lazy(epsEqual 1e-15 ((max v axis).ToArray()) res))

    [<Property>]
    let ``mean``(v : float[,]) (axis : MatrixAxis) =
        let v = new Matrix(v)
        match axis with
            | ColumnAxis -> setMatrix app "v" (v.ToArray2D())
            | RowAxis -> setMatrix app "v" (transpose(v).ToArray2D())
        app.Execute("res = mean(v);") |> ignore
        let res = getVector app "res"
        ((v.RowCount > 1 && v.ColCount > 1) ==> lazy(epsEqual 1e-15 ((mean v axis).ToArray()) res))

    [<Property>]
    let ``var``(v : float[,]) (axis : MatrixAxis) =
        let v = v |> Array2D.map (fun x -> if Double.IsInfinity(x) || abs(x) = Double.MaxValue || abs(x) = Double.Epsilon then 1.0 else x)
        let v = new Matrix(v)
        match axis with
            | ColumnAxis -> setMatrix app "v" (v.ToArray2D())
            | RowAxis -> setMatrix app "v" (transpose(v).ToArray2D())
        app.Execute("res = var(v);") |> ignore
        let res = getVector app "res" 
        ((v.RowCount > 1 && v.ColCount > 1) ==> lazy(epsEqual 1e-10 ((var v axis).ToArray()) res))

    [<Property>]
    let ``skewness``(v : float[,]) (axis : MatrixAxis) =
        let v = v |> Array2D.map (fun x -> if Double.IsNaN(x) || Double.IsInfinity(x) || abs(x) = Double.MaxValue || abs(x) = Double.Epsilon then 1.0 else x)
        let v = new Matrix(v)
        match axis with
            | ColumnAxis -> setMatrix app "v" (v.ToArray2D())
            | RowAxis -> setMatrix app "v" (transpose(v).ToArray2D())
        app.Execute("res = skewness(v);") |> ignore
        let res = getVector app "res"
        ((v.RowCount > 1 && v.ColCount > 1) ==> lazy(epsEqual 1e-5 ((skewness v axis).ToArray()) res))

    [<Property>]
    let ``kurtosis``(v : float[,]) (axis : MatrixAxis) =
        let v = v |> Array2D.map (fun x -> if Double.IsNaN(x) || Double.IsInfinity(x) || abs(x) = Double.MaxValue || abs(x) = Double.Epsilon then 1.0 else x)
        let v = new Matrix(v)
        match axis with
            | ColumnAxis -> setMatrix app "v" (v.ToArray2D())
            | RowAxis -> setMatrix app "v" (transpose(v).ToArray2D())
        app.Execute("res = kurtosis(v);") |> ignore
        let res = getVector app "res"
        ((v.RowCount > 1 && v.ColCount > 1) ==> lazy(epsEqual 1e-5 ((kurtosis v axis).ToArray()) res))

    [<Fact>]
    let ``quantile ColumnAxis``()  =
        let axis = ColumnAxis
        use m =
            match axis with
                | RowAxis -> unifRnd rng 0.0 1000.0 5L 10000000L
                | ColumnAxis -> unifRnd rng 0.0 1000.0 10000000L 5L 
        let q = new Vector([|1.0..(-0.1)..0.0|])
        let resArr = [[|1000.0..(-100.)..0.0|]
                      [|1000.0..(-100.)..0.0|]
                      [|1000.0..(-100.)..0.0|]
                      [|1000.0..(-100.)..0.0|]
                      [|1000.0..(-100.)..0.0|]]
        let res = 
            match axis with
                | RowAxis -> new Matrix(resArr)
                | ColumnAxis -> transpose(new Matrix(resArr))
        epsEqual2D 1e-2 ((quantile m q axis).ToArray2D()) (res.ToArray2D()) |> should be True

    [<Fact>]
    let ``quantile RowAxis``()  =
        let axis = RowAxis
        use m =
            match axis with
                | RowAxis -> unifRnd rng 0.0 1000.0 5L 10000000L
                | ColumnAxis -> unifRnd rng 0.0 1000.0 10000000L 5L 
        let q = new Vector([|1.0..(-0.1)..0.0|])
        let resArr = [[|1000.0..(-100.)..0.0|]
                      [|1000.0..(-100.)..0.0|]
                      [|1000.0..(-100.)..0.0|]
                      [|1000.0..(-100.)..0.0|]
                      [|1000.0..(-100.)..0.0|]]
        let res = 
            match axis with
                | RowAxis -> new Matrix(resArr)
                | ColumnAxis -> transpose(new Matrix(resArr))
        epsEqual2D 1e-2 ((quantile m q axis).ToArray2D()) (res.ToArray2D()) |> should be True

    [<Property>]
    let ``cov``(v : float[,]) =
        let v = v |> Array2D.map (fun x -> if Double.IsInfinity(x) || abs(x) = Double.MaxValue || abs(x) = Double.Epsilon then 1.0 else x)
        setMatrix app "v" v
        app.Execute("res = cov(v);") |> ignore
        let res = getMatrix app "res"
        let v = new Matrix(v)
        ((v.RowCount > 1 && v.ColCount > 1) ==> lazy(epsEqual2D 1e-10 ((cov v).ToArray2D()) res))

    [<Property>]
    let ``corr``(v : float[,]) =
        let v = v |> Array2D.map (fun x -> if Double.IsNaN(x) then 1.0 elif Double.IsInfinity(x) then 2.0 elif x = Double.MaxValue then 3.0 elif x = Double.MinValue then 4.0 elif abs(x) = Double.Epsilon then 5.0 else x)
        setMatrix app "v" v
        app.Execute("res = corr(v);") |> ignore
        let res = getMatrix app "res"
        let v = new Matrix(v)
        ((v.RowCount > 1 && v.ColCount > 1) ==> lazy(epsEqual2D 1e-8 ((corr v).ToArray2D()) res))

