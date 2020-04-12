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
  
    type thing = {
        a: string
    }

    type GestureEventData = import('tns-core-modules/ui/gestures').GestureEventData;

    [[INTRINSIC_ELEMENTS]]
    
}