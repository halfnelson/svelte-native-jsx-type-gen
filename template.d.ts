declare namespace svelteNative.JSX {
 
    /* svelte2tsx JSX */
    interface ElementClass {
      $$prop_def: any;
    }
  
    interface ElementAttributesProperty {
      $$prop_def: any; // specify the property name to use
    }
  
    /* svelte-native jsx types */
    type SvelteNativeNode = {};
  
    export type Child = SvelteNativeNode | SvelteNativeNode[] | string | number;
    export type Children = Child | Child[];
  
    interface IntrinsicAttributes {}

    [[INTRINSIC_ELEMENTS]]
    
}