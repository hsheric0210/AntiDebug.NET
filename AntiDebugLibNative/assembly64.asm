.code 

__int2d_64 proc
    xor rax, rax
    int 2dh
    nop
    ret
__int2d_64 endp

__icebp_64 proc
    xor rax, rax
    int 1 ; F1
    nop
    ret
__icebp_64 endp

__popf_trap_64 proc
    xor rax, rax
    pushfq
    mov qword ptr [rsp], 100h
    popfq
    nop
    ret
__popf_trap_64 endp

__div0_64 proc
    xor rax, rax
    div rax
    nop
    ret
__div0_64 endp

__unhandled_exception_filter_64 proc
    xor rax, rax
    int 3 ; CC
    inc rax ; 48 FF C0
    ret
    ret
    ret
__unhandled_exception_filter_64 endp

end

