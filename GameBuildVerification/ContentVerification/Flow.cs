using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace ContentVerification
{
	/// <summary>
	/// Hgx has very specific data-flows for the following 3 scenarios:
	/// - Hash    (reading from files)
	/// - Commit  (reading from files, writing to bins)
	/// - Update  (reading from bins, writing to files)
	/// </summary>
	public enum EFlowState : int
	{
		READ = 0,
		WRITE = 1,
		GATHER = 2,
		END = 3,
		WORK = 4,
	}

	public interface IFlowActor
	{
		EFlowState Transition { get; set; }
		void Execute(EFlowState flow);
	}

	public static class Flow
	{
		public class Buffer
		{
			public byte[] Data { get; set; }
			public int Size { get; set; }

			public bool IsNull { get { return Data == null || Size == 0; } }
			public bool IsNotNull { get { return Data != null || Size > 0; } }

			public Buffer()
			{
				Data = null;
				Size = 0;
			}

			public Buffer(byte[] data, int size)
			{
				Data = data;
				Size = size;
			}

			public void Set(Buffer b)
			{
				Data = b.Data;
				Size = b.Size;
			}

			public void Clear()
			{
				Size = 0;
				Data = null;
			}

			public void Reset()
			{
				Size = 0;
			}
		}

		public class Worker
		{
			private Thread mThread;

			public Worker(string name, EFlowState flow, BlockingCollection<IFlowActor>[] pipes)
			{
				Name = name;
				Flow = flow;
				Work = pipes[(int)flow];
				Pipes = pipes;
			}

			public string Name { get; set; }
			public EFlowState Flow { get; set; }
			public BlockingCollection<IFlowActor> Work { get; set; }
			public BlockingCollection<IFlowActor>[] Pipes { get; set; }

			public void Start()
			{
				mThread = new Thread(DoWork);
				mThread.Start();
			}

			public void Stop()
			{
				mThread.Join();
			}

			private void DoWork()
			{
				while (true)
				{
					IFlowActor main;
					if (!Work.TryTake(out main, Timeout.Infinite))
						break;

					/// Terminator ?
					if (main == null)
						break;

					///if (main.Transition == EPipe.WORK)
					///	Console.WriteLine("work: {0}", Name);

					main.Execute(Flow);
					Pipes[(int)main.Transition].Add(main);
				}
			}
		}

		public class Engine
		{
			private Worker[] Threads;
			private BlockingCollection<IFlowActor>[] Pipes;
			private List<EFlowState> PipeTypes;

			public static void Init()
			{
				Engine engine = new Engine(Environment.ProcessorCount - 2);
			}

			public Engine(int workers)
			{
				PipeTypes = new List<EFlowState>() { EFlowState.READ, EFlowState.WRITE, EFlowState.GATHER, EFlowState.END, EFlowState.WORK };

				Pipes = new BlockingCollection<IFlowActor>[PipeTypes.Count];
				for (int i = 0; i < PipeTypes.Count; ++i)
					Pipes[i] = new BlockingCollection<IFlowActor>(new ConcurrentQueue<IFlowActor>(), 128);

				PipeTypes.Remove(EFlowState.END);
				PipeTypes.Remove(EFlowState.WORK);
				for (int i = 0; i < workers; ++i)
					PipeTypes.Add(EFlowState.WORK);

				Threads = new Worker[PipeTypes.Count];
				for (int i = 0; i < PipeTypes.Count; ++i)
					Threads[i] = new Worker(PipeTypes[i].ToString(), PipeTypes[i], Pipes);
			}

			public void Execute(List<IFlowActor> actors)
			{
				foreach (Worker thread in Threads)
					thread.Start();

				/// Push the actors on the flows
				foreach (IFlowActor actor in actors)
					Threads[(int)actor.Transition].Work.Add(actor);

				/// Wait until all actors have arrived in END
				int num_actors = actors.Count;
				for (int i = 0; i < num_actors; ++i)
				{
					IFlowActor actor;
					Pipes[(int)EFlowState.END].TryTake(out actor, Timeout.Infinite);
				}

				for (int i = 0; i < PipeTypes.Count; ++i)
				{
					int p = (int)PipeTypes[i];
					Pipes[p].Add(null);
				}

				foreach (Worker thread in Threads)
					thread.Stop();
			}
		}
	}
}
