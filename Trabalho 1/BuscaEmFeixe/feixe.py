#!/usr/bin/env python3

import json
import argparse
import cProfile
from bitarray import bitarray

class Estado():
    def __init__(self, vet, peso=0):
        self.vet = vet
        self.peso = peso
    def include(self, ii):
        vet = self.vet.copy()
        vet[ii] = True
        return Estado(vet, peso=self.peso+itens[ii])
    def valido(self):
        return self.peso <= T

# Parametros
def main():
    global itens, k, T
    parser = argparse.ArgumentParser()
    parser.add_argument("k", type=int)
    parser.add_argument("T", type=int)
    parser.add_argument('itens', type=argparse.FileType('r'))
    parser.add_argument('outcsv', type=argparse.FileType('w'))
    args = parser.parse_args()
    itens = json.loads("[" + args.itens.read() + "]")
    k = args.k
    T = args.T
    print(k, T, len(itens))
#   cProfile.run("run()")
    run()

def selecionar_k_estados(estados):
    a = sorted(estados, key=lambda x: x.peso, reverse=True)
    a =  a[:k]
    print("best:", a[0].peso)
    return a

def sucessoras(estado, acc):
    #print("s1", estado)
    acc.add(estado)
    for ii, dentro in enumerate(estado.vet):
        #print("s2", ii, dentro)
        if not dentro:
            temp = estado.include(ii)
            if temp.valido():
                acc.add(temp)

def run():
    aestados = [Estado(bitarray([False] * len(itens)))]
    anterior = aestados

    import itertools
    for i in itertools.count():
        print(i, "A", len(aestados))
        acc = set()
        for estado in aestados:
            print("A0-1")
            succ = sucessoras(estado, acc)
            print("A0-2")
            print("A0-3")
        print(i, "A1")
        aestados = selecionar_k_estados(acc)
        print(i, "B", len(aestados))

        #print("2", aestados)
        
        resp_max = max(aestados, key=lambda x: x.peso).peso
        print("max:", resp_max)
        if aestados == anterior:
            acc = set()
            for estado in aestados:
                if estado.peso == resp_max:
                    acc.add(estado)
            print("fim:")
            for estado in acc:
                print("es:", estado.vet, estado.peso)
            break

        print(i, "C")
        anterior = aestados


main()
