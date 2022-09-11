#!/usr/bin/env python3


# Parametros
itens = [1, 1, 2, 5, 4, 3, 3, 2, 6, 10]
k = 3
T = 25


aestados = [tuple([False] * len(itens))]

def selecionar_k_estados(estados):
    a = [(estado, peso(estado)) for estado in estados]
    a = sorted(a, key=lambda x: x[1], reverse=True)
    a =  a[:k]
    return [aa[0] for aa in a]

def peso(estado):
    peso = 0
    for ii, dentro in enumerate(estado):
        if dentro:
            peso += itens[ii]
    return peso

def estado_valido(estado):
    return peso(estado) <= T

def sucessoras(estado):
    print("s1", estado)
    acc = set()
    acc.add(estado)
    for ii, dentro in enumerate(estado):
        print("s2", ii, dentro)
        if not dentro:
            temp = list(estado)
            temp[ii] = True
            if estado_valido(temp):
                acc.add(tuple(temp))
    return acc

anterior = aestados

while True:
    print("1", aestados)
    acc = set()
    for estado in aestados:
        acc = acc.union(sucessoras(estado))
    aestados = selecionar_k_estados(acc)

    print("2", aestados)
    
    if aestados == anterior:
        a = [(estado, peso(estado)) for estado in aestados]
        resp_max = max(a, key=lambda x: x[1])[1]
        acc = set()
        for estado, pes in a:
            if pes == resp_max:
                acc.add(estado)
        print("fim:", acc)
        break

    anterior = aestados

