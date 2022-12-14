#!/usr/bin/env python3

import json
import argparse
import cProfile
import csv
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
def parser_main():
    global itens, k, T, M
    parser = argparse.ArgumentParser()
    parser.add_argument("k", type=int)
    parser.add_argument("T", type=int)
    parser.add_argument("M", type=int)
    parser.add_argument('itens', type=argparse.FileType('r'))
    parser.add_argument('outcsv', type=argparse.FileType('w'))
    args = parser.parse_args()
    itens = json.loads("[" + args.itens.read() + "]")
    k = args.k
    T = args.T
    M = args.M
    print(k, T, M, len(itens))
#   cProfile.run("run()")
    run()

def range_main():
    global itens, k, T, M, csvwriter
    parser = argparse.ArgumentParser()
    parser.add_argument('itens', type=argparse.FileType('r'))
    parser.add_argument('outcsv', type=argparse.FileType('w'))
    args = parser.parse_args()
    all_itens = json.loads("[" + args.itens.read() + "]")
    csvwriter = csv.writer(args.outcsv)
    csvwriter.writerow(["k", "T", "M", "num items", "loops", "Resultado", "Ideal"])
    for k in [1, 3, 5, 20, 50]:
        for T in [273, 5629, 31381]:
            args.outcsv.flush()
            for M in [10]:
                for num_itens in [100, 1000, 10000]:
                    itens = all_itens[:num_itens]
                    print(k, T, M, len(itens))
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
    anterior = 0
    contador_sem_progresso = 0

    import itertools
    for i in itertools.count():
        print(i, "A", len(aestados))
        acc = set()
        for estado in aestados:
            succ = sucessoras(estado, acc)
        aestados = selecionar_k_estados(acc)
        print(i, "B", len(aestados))

        #print("2", aestados)
        
        resp_max = max(aestados, key=lambda x: x.peso).peso
        print("max:", resp_max)
        if resp_max == anterior:
            contador_sem_progresso += 1
        else:
            contador_sem_progresso = 0
        if contador_sem_progresso == M:
            acc = set()
            for estado in aestados:
                if estado.peso == resp_max:
                    acc.add(estado)
            print("fim:", resp_max)

            csvwriter.writerow([k, T, M, len(itens), i, resp_max, resp_max == T])
            break

        print(i, "C")
        anterior = resp_max


range_main()
