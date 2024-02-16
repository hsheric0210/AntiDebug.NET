.code 

__int2d_64 proc
    xor rax, rax
    int 2dh
    nop
    ret
__int2d_64 endp

__icebp_64 proc
    xor rax, rax
    int 1
    nop
    ret
__icebp_64 endp

__popf_trap proc
    xor rax, rax
    pushfq
    mov qword ptr [rsp], 100h
    popfq
    nop
    ret
__popf_trap endp

__div0_64 proc
    xor rax, rax
    div eax
    nop
    ret
__div0_64 endp

end

