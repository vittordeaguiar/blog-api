import { motion } from "framer-motion";

export function LoadingProgress() {
  return (
    <motion.div
      className="fixed top-0 left-0 right-0 h-1 bg-brand z-50 origin-left"
      initial={{ scaleX: 0 }}
      animate={{ scaleX: 1 }}
      transition={{
        duration: 1.5,
        ease: "easeInOut",
        repeat: Infinity,
        repeatType: "loop",
      }}
    />
  );
}
