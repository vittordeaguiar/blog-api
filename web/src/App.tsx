import { useState } from "react";
import { Button } from "./shared/ui/button";

function App() {
  const [count, setCount] = useState(0);

  return (
    <div className="w-screen flex flex-col items-center justify-center space-y-4 p-4">
      <h1 className="text-3xl font-semibold underline">Web Layer</h1>
      <div className="card">
        <Button onClick={() => setCount((count) => count + 1)}>count is {count}</Button>
      </div>
    </div>
  );
}

export default App;
