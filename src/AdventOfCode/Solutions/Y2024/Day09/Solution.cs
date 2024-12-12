using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions.Y2024.Day09;

public class Solution(IOptions<AppSettings> options, IFileReader fileReader) : BaseSolution(options, fileReader)
{
    public override long GetSolution_1(string fileName)
    {
        List<int> disk = _fileReader.Read(GetFullFilePath(fileName)).Select(c => int.Parse(c.ToString())).ToList();

        int currentFileIndex = 0;
        int nFiles = 0;

        while(currentFileIndex < disk.Count)
        {
            if (disk[currentFileIndex] > 0)
            {
                nFiles++;
            }

            currentFileIndex += 2;
        }

        int i = 0;
        int blockPosition = 0;
        int remainingBlocks;
        long output = 0;

        int leftFileId = 0;
        int leftFileIndex = 0;

        while (leftFileIndex < disk.Count && disk[leftFileIndex] == 0)
        {
            leftFileIndex += 2;
        }

        if(leftFileIndex >= disk.Count)
        {
            return 0;
        }

        int rightFileId = nFiles-1;

        int rightFileIndex;

        if(disk.Count % 2 == 0)
        {
            rightFileIndex = disk.Count - 2;
        }
        else
        {
            rightFileIndex = disk.Count - 1;
        }

        while(rightFileIndex >= 0 && disk[rightFileIndex] == 0)
        {
            rightFileIndex -= 2;
        }

        int currentFreeBlocks;
        bool isFile = true;

        while(i < disk.Count &&
            leftFileIndex <= rightFileIndex &&
            disk[leftFileIndex] > 0 &&
            disk[rightFileIndex] > 0)
        {
            if(isFile)
            {
                remainingBlocks = disk[leftFileIndex];
                while (remainingBlocks > 0)
                {
                    output += blockPosition * leftFileId;
                    blockPosition++;
                    remainingBlocks--;
                }
                leftFileId++;
                leftFileIndex += 2;
            }
            else
            {
                currentFreeBlocks = disk[i];
                remainingBlocks = disk[rightFileIndex];

                while (currentFreeBlocks > 0 && remainingBlocks > 0)
                {
                    output += blockPosition * rightFileId;
                    blockPosition++;
                    currentFreeBlocks--;
                    remainingBlocks--;
                }

                disk[i] = currentFreeBlocks;
                disk[rightFileIndex] = remainingBlocks;

                if (remainingBlocks == 0)
                {
                    rightFileIndex -= 2;
                    rightFileId--;
                }

                if (currentFreeBlocks > 0)
                {
                    continue;
                }
            }
            i++;
            isFile = !isFile;
        }


        return output;
    }

    public override long GetSolution_2(string fileName)
    {
        List<int> disk = _fileReader.Read(GetFullFilePath(fileName)).Select(c => int.Parse(c.ToString())).ToList();

        int currentFileIndex = 0;
        int nFiles = 0;

        while (currentFileIndex < disk.Count)
        {
            if (disk[currentFileIndex] > 0)
            {
                nFiles++;
            }

            currentFileIndex += 2;
        }

        long output = 0;

        int rightFileId = nFiles - 1;

        int rightFileIndex;

        if (disk.Count % 2 == 0)
        {
            rightFileIndex = disk.Count - 2;
        }
        else
        {
            rightFileIndex = disk.Count - 1;
        }

        Dictionary<int, int> indexToBlockStart = [];
        int currentFileId = rightFileId;

        int currentBlock = 0;
        for(int i = 0; i < disk.Count; i++)
        {
            indexToBlockStart.Add(i, currentBlock);
            currentBlock += disk[i];
        }

        currentFileIndex = rightFileIndex;

        int remainingBlocks;

        
        int key;



        while (currentFileId >= 0)
        {
            if (disk[currentFileIndex] == 0)
            {
                currentFileIndex -= 2;
                continue;
            }

            remainingBlocks = disk[currentFileIndex];
            key = 1;

            while(key < currentFileIndex)
            {
                if (disk[key] >= remainingBlocks)
                {
                    for (int i = indexToBlockStart[key]; i < indexToBlockStart[key] + remainingBlocks; i++)
                    {
                        output += currentFileId * i;
                    }

                    disk[key] -= remainingBlocks;
                    indexToBlockStart[key] += remainingBlocks;

                    break;
                }

                key += 2;
            }

            if(key >= currentFileIndex)
            {
                for (int i = indexToBlockStart[currentFileIndex]; i < indexToBlockStart[currentFileIndex] + remainingBlocks; i++)
                {
                    output += currentFileId * i;
                }
            }

            currentFileId--;
            currentFileIndex -= 2;
        }


        return output;
    }
}
