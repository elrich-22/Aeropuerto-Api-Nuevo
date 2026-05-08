import { useMemo } from 'react';

export default
function Stars() {
  const stars = useMemo(
    () =>
      Array.from({ length: 95 }, (_, index) => ({
        id: index,
        size: `${Math.random() * 2.2 + 0.6}px`,
        top: `${Math.random() * 100}%`,
        left: `${Math.random() * 100}%`,
        duration: `${2 + Math.random() * 4}s`,
        delay: `${Math.random() * 5}s`
      })),
    []
  );
}